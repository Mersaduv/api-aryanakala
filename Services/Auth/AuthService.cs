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

        public AuthService(IDistributedCache cache, IOptions<Configs> options, ApplicationDbContext applicationDbContext,
        IUnitOfWork unitOfWork, EncryptionUtility encryptionUtility)
        {
            _cache = cache;
            configs = options.Value;
            this.applicationDbContext = applicationDbContext;
            this.unitOfWork = unitOfWork;
            this.encryptionUtility = encryptionUtility;
        }


        public async Task<GenerateNewTokenDTO> GenerateNewToken(GenerateNewTokenDTO command)
        {
            var userRefreshToken = await applicationDbContext.UserRefreshTokens
            .SingleOrDefaultAsync(q => q.RefreshToken == command.RefreshToken);

            var userId = await ValidateRefreshToken(command.RefreshToken);
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null) throw new CoreException("User not found");
            if (userRefreshToken is null) throw new CoreException("RefreshToken not found");

            var token = GetNewToken(userRefreshToken.UserId);
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

            return result;
        }

        public async Task Register(LoginRequestDTO command)
        {
            var salt = encryptionUtility.GetNewSalt();
            var hashPassowrd = encryptionUtility.GetSHA256(command.Password, salt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Password = hashPassowrd,
                PasswordSalt = salt,
                RegisterDate = DateTime.UtcNow,
                UserName = command.UserName
            };

            await applicationDbContext.Users.AddAsync(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO command)
        {
            var user = await applicationDbContext.Users.AsNoTracking().FirstOrDefaultAsync(q => q.UserName == command.UserName);
            if (user is null)
            {
                throw new CoreException("Username is incorrect");
            }
            var hashPassowrd = encryptionUtility.GetSHA256(command.Password, user.PasswordSalt);

            // Check passWord
            if (user.Password != hashPassowrd) throw new CoreException("Password is incorrect");

            var token = GetNewToken(user.Id);
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

            var result = new LoginResponseDTO
            {
                UserName = user.UserName,
                Token = token,
                ExpireTime = configs.TokenTimeout,
                RefreshToken = refreshToken,
                RefreshTokenExpireTime = configs.RefreshTokenTimeout
            };

            return result;
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

    }

}