using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ApiAryanakala.Entities.Exceptions;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ApiAryanakala.Entities.Exceptions;
using ApiAryanakala.Data;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Utility;
using Microsoft.EntityFrameworkCore;
using ApiAryanakala.Entities;
using ApiAryanakala.Interfaces.IServices;

namespace ApiAryanakala.Services.Auth
{
    public class AuthService : IAuthServices
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;

        private readonly Configs configs;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IUnitOfWork unitOfWork;


        private readonly EncryptionUtility encryptionUtility;
        private readonly IHttpContextAccessor httpContextAccessor;


        public AuthService(IDistributedCache cache, IOptions<Configs> options, ApplicationDbContext applicationDbContext,
        IUnitOfWork unitOfWork, EncryptionUtility encryptionUtility, IHttpContextAccessor httpContextAccessor)
        {
            _cache = cache;
            configs = options.Value;
            this.applicationDbContext = applicationDbContext;
            this.unitOfWork = unitOfWork;
            this.encryptionUtility = encryptionUtility;
            this.httpContextAccessor = httpContextAccessor;

        }


        public async Task<ServiceResponse<GenerateNewTokenDTO>> GenerateNewToken(GenerateNewTokenDTO command)
        {
            var userRefreshToken = await applicationDbContext.UserRefreshTokens
            .SingleOrDefaultAsync(q => q.RefreshToken == command.RefreshToken);

            var userId = await ValidateRefreshToken(command.RefreshToken);
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
            {
                return new ServiceResponse<GenerateNewTokenDTO>()
                {
                    Data = null,
                    Success = false,
                    Message = "User not found"
                };
            }
            if (userRefreshToken is null)
            {
                return new ServiceResponse<GenerateNewTokenDTO>()
                {
                    Data = null,
                    Success = false,
                    Message = "RefreshToken not found"
                };
            }

            var token = CreateToken(user);
            var refreshToken = await GenerateRefreshToken(userRefreshToken.UserId);

            // Insert or update refresh token in db
            var currentRefreshToken = await applicationDbContext.UserRefreshTokens
                .SingleOrDefaultAsync(q => q.UserId == userRefreshToken.UserId);

            if (currentRefreshToken is null)
            {
                // If there is no existing refresh token, create a new one
                currentRefreshToken = new UserRefreshToken
                {
                    UserId = userRefreshToken.UserId,
                    RefreshToken = refreshToken,
                    RefreshTokenTimeout = configs.RefreshTokenTimeout,
                    CreateDate = DateTime.UtcNow,
                    IsValid = true
                };

                await applicationDbContext.UserRefreshTokens.AddAsync(userRefreshToken);
            }
            else
            {
                // If there is an existing refresh token, update its values
                currentRefreshToken.RefreshToken = refreshToken;
                currentRefreshToken.RefreshTokenTimeout = configs.RefreshTokenTimeout;
                currentRefreshToken.CreateDate = DateTime.UtcNow;
                currentRefreshToken.IsValid = true;
            }

            // Save changes to the database
            await unitOfWork.SaveChangesAsync();

            var result = new GenerateNewTokenDTO
            {
                Token = token,
                RefreshToken = refreshToken
            };

            return new ServiceResponse<GenerateNewTokenDTO>
            {
                Data = result
            };
        }

        public async Task<ServiceResponse<Guid>> RegisterAsync(User user, string password)
        {
            // var salt = encryptionUtility.GetNewSalt();
            // var hashPassowrd = encryptionUtility.GetSHA256(command.Password, salt);
            if (await UserExistsAsync(user.UserName))
            {
                return new ServiceResponse<Guid> { Success = false, Message = "User already exists." };
            }

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.Password = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.RegisterDate = DateTime.UtcNow;


            await applicationDbContext.Users.AddAsync(user);
            await unitOfWork.SaveChangesAsync();

            return new ServiceResponse<Guid> { Data = user.Id };
        }

        public async Task<ServiceResponse<LoginResponse>> LogInAsync(string email, string password)
        {
            var user = await applicationDbContext.Users.AsNoTracking().FirstOrDefaultAsync(q => q.Email.ToLower() == email.ToLower());
            var response = new ServiceResponse<LoginResponse>();
            if (user is null)
            {
                response.Success = false;
                response.Message = "User not found.";
            }
            else if (!VerifyPasswordHash(password, user.Password, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password.";
            }

            var token = CreateToken(user);
            var refreshToken = await GenerateRefreshToken(user.Id);

            // Insert or update refresh token in db
            var userRefreshToken = await applicationDbContext.UserRefreshTokens
                .SingleOrDefaultAsync(q => q.UserId == user.Id);

            if (userRefreshToken == null)
            {
                // If there is no existing refresh token, create a new one
                userRefreshToken = new UserRefreshToken
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    RefreshTokenTimeout = configs.RefreshTokenTimeout,
                    CreateDate = DateTime.UtcNow,
                    IsValid = true
                };

                await applicationDbContext.UserRefreshTokens.AddAsync(userRefreshToken);
            }
            else
            {
                // If there is an existing refresh token, update its values
                userRefreshToken.RefreshToken = refreshToken;
                userRefreshToken.RefreshTokenTimeout = configs.RefreshTokenTimeout;
                userRefreshToken.CreateDate = DateTime.UtcNow;
                userRefreshToken.IsValid = true;
            }

            // Save changes to the database
            await unitOfWork.SaveChangesAsync();

            var result = new LoginResponse
            {
                UserName = user.UserName,
                Token = token,
                ExpireTime = configs.TokenTimeout,
                RefreshToken = refreshToken,
                RefreshTokenExpireTime = configs.RefreshTokenTimeout
            };
            response.Data = result;

            return response;
        }

        public async Task<ServiceResponse<bool>> ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await applicationDbContext.Users.FindAsync(userId);
            if (user is null)
            {
                return new ServiceResponse<bool> { Success = false, Message = "User not found.", Data = false };
            }

            CreatePasswordHash(newPassword, out var passwordHash, out var passwordSalt);

            user.Password = passwordHash;
            user.PasswordSalt = passwordSalt;
            await unitOfWork.SaveChangesAsync();

            return new ServiceResponse<bool> { Success = true, Message = "Password has been changed.", Data = true };
        }


        private string GetNewToken(Guid userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configs.TokenKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                            new Claim("userId", userId.ToString()),
                }),

                Expires = DateTime.Now.AddMinutes(configs.TokenTimeout),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string> GenerateRefreshToken(Guid userId)
        {
            var randomNumber = new byte[32];
            var refreshToken = "";
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken = Convert.ToBase64String(randomNumber);
            }
            var userCache = new UserCacheDto { UserId = userId };
            var serializedUser = JsonConvert.SerializeObject(userCache);
            var cacheData = Encoding.UTF8.GetBytes(serializedUser);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(configs.RefreshTokenTimeout)
            };
            await _cache.SetAsync(refreshToken, cacheData, cacheOptions);
            return refreshToken;
        }

        private async Task<Guid> ValidateRefreshToken(string refreshToken)
        {
            var redisUser = await _cache.GetAsync(refreshToken);
            if (redisUser is null)
            {
                throw new CoreException("Invalid Refresh Token");
            }
            var serializedUser = Encoding.UTF8.GetString(redisUser);
            var userCache = JsonConvert.DeserializeObject<UserCacheDto>(serializedUser);
            return userCache.UserId;
        }

        public Task<ServiceResponse<bool>> ChangePassword(Guid userId, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Guid GetUserId() => Guid.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public string GetUserEmail() => httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);



        public async Task<bool> UserExists(string email)
        {
            if (await applicationDbContext.Users.AnyAsync(user => user.Email.ToLower()
                 .Equals(email.ToLower())))
            {
                return true;
            }
            return false;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await applicationDbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private async Task<bool> UserExistsAsync(string email) =>
         await applicationDbContext.Users.AnyAsync(user => user.Email.ToLower() == email.ToLower());


        private static bool VerifyPasswordHash(string password, IEnumerable<byte> passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.Email, user.Email),
        };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configs.TokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(claims: claims,
                expires: DateTime.Now.AddMinutes(configs.TokenTimeout), signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }

}