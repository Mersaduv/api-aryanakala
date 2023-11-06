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

namespace ApiAryanakala.Services.Auth
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;

        private readonly Configs configs;

        public AuthService(IDistributedCache cache, IOptions<Configs> options)
        {
            _cache = cache;
            this.configs = options.Value;

        }



        public string GetNewToken(Guid userId)
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

        public async Task<string> GenerateRefreshToken(Guid userId)
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

        public async Task<Guid> ValidateRefreshToken(string refreshToken)
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