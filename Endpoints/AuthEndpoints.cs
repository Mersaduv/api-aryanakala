using System.Net;
using ApiAryanakala.Data;
using ApiAryanakala.Entities;
using ApiAryanakala.Entities.Exceptions;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO;
using ApiAryanakala.Services.Auth;
using ApiAryanakala.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace ApiAryanakala.Endpoints
{
    public static class AuthEndpoints
    {
        public static void ConfigureAuthEndpoints(this WebApplication app)
        {

            app.MapPost("/api/login", Login).WithName("Login").Accepts<LoginRequestDTO>("application/json")
                .Produces<APIResponse>(200).Produces(400);
            app.MapPost("/api/register", Register).WithName("Register").Accepts<RegisterationRequestDTO>("application/json")
                .Produces<APIResponse>(200).Produces(400);
            app.MapPost("/api/generateToken", GenerateNewToken).WithName("GenerateNewToken").Accepts<GenerateNewTokenDTO>("application/json").Produces<APIResponse>(200).Produces(400); ;
        }

        private async static Task<IResult> GenerateNewToken(ApplicationDbContext applicationDbContext, GenerateNewTokenDTO model,
         EncryptionUtility encryptionUtility,
         IUnitOfWork unitOfWork, IOptions<Configs> options, AuthService _authService)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            var userRefreshToken = await applicationDbContext.UserRefreshTokens
            .SingleOrDefaultAsync(q => q.RefreshToken == model.RefreshToken);

            var userId = await _authService.ValidateRefreshToken(model.RefreshToken);
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null) throw new CoreException("User not found");
            if (userRefreshToken is null) throw new CoreException("RefreshToken not found");

            var token = _authService.GetNewToken(userRefreshToken.UserId);
            var refreshToken = await _authService.GenerateRefreshToken(userRefreshToken.UserId);

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
                    RefreshTokenTimeout = options.Value.RefreshTokenTimeout,
                    CreateDate = DateTime.UtcNow,
                    IsValid = true
                };

                await applicationDbContext.UserRefreshTokens.AddAsync(userRefreshToken);
            }
            else
            {
                // If there is an existing refresh token, update its values
                currentRefreshToken.RefreshToken = refreshToken;
                currentRefreshToken.RefreshTokenTimeout = options.Value.RefreshTokenTimeout;
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

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = result;
            return Results.Ok(response);
        }


        private async static Task<IResult> Register(IUnitOfWork unitOfWork, LoginRequestDTO model, EncryptionUtility encryptionUtility, ApplicationDbContext applicationDbContext)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            var salt = encryptionUtility.GetNewSalt();
            var hashPassowrd = encryptionUtility.GetSHA256(model.Password, salt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Password = hashPassowrd,
                PasswordSalt = salt,
                RegisterDate = DateTime.UtcNow,
                UserName = model.UserName
            };

            await applicationDbContext.Users.AddAsync(user);
            await unitOfWork.SaveChangesAsync();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }


        private async static Task<IResult> Login(IUnitOfWork unitOfWork, ApplicationDbContext applicationDbContext,
            [FromBody] LoginRequestDTO model, EncryptionUtility encryptionUtility, IOptions<Configs> options, AuthService _authService)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            var user = await applicationDbContext.Users.AsNoTracking().FirstOrDefaultAsync(q => q.UserName == model.UserName);
            if (user is null)
            {
                throw new CoreException("Username is incorrect");
            }
            var hashPassowrd = encryptionUtility.GetSHA256(model.Password, user.PasswordSalt);

            // Check passWord
            if (user.Password != hashPassowrd) throw new CoreException("Password is incorrect");

            var token = _authService.GetNewToken(user.Id);
            var refreshToken = await _authService.GenerateRefreshToken(user.Id);

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
                    RefreshTokenTimeout = options.Value.RefreshTokenTimeout,
                    CreateDate = DateTime.UtcNow,
                    IsValid = true
                };

                await applicationDbContext.UserRefreshTokens.AddAsync(userRefreshToken);
            }
            else
            {
                // If there is an existing refresh token, update its values
                userRefreshToken.RefreshToken = refreshToken;
                userRefreshToken.RefreshTokenTimeout = options.Value.RefreshTokenTimeout;
                userRefreshToken.CreateDate = DateTime.UtcNow;
                userRefreshToken.IsValid = true;
            }

            // Save changes to the database
            await unitOfWork.SaveChangesAsync();

            var result = new LoginResponseDTO
            {
                UserName = user.UserName,
                Token = token,
                ExpireTime = options.Value.TokenTimeout,
                RefreshToken = refreshToken,
                RefreshTokenExpireTime = options.Value.RefreshTokenTimeout
            };

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = result;
            return Results.Ok(response);
        }


    }
}

