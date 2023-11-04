using System.Net;
using ApiAryanakala.Data;
using ApiAryanakala.Entities;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO;
using ApiAryanakala.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace ApiAryanakala.Endpoints;

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
     IUnitOfWork unitOfWork, IOptions<Configs> options)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
        var userRefreshToken = await applicationDbContext.UserRefreshTokens
        .SingleOrDefaultAsync(q => q.RefreshToken == model.RefreshToken);


        if (userRefreshToken == null) throw new Exception();

        var token = encryptionUtility.GetNewToken(userRefreshToken.UserId);
        var refreshToken = encryptionUtility.GetNewRefreshToken();

        // Insert or update refresh token in db
        var currentRefreshToken = await applicationDbContext.UserRefreshTokens
            .SingleOrDefaultAsync(q => q.UserId == userRefreshToken.UserId);

        if (currentRefreshToken == null)
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
            RegisterDate = DateTime.Now,
            UserName = model.UserName
        };

        await applicationDbContext.Users.AddAsync(user);
        await unitOfWork.SaveChangesAsync();

        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }


    private async static Task<IResult> Login(IUnitOfWork unitOfWork, ApplicationDbContext applicationDbContext,
        [FromBody] LoginRequestDTO model, EncryptionUtility encryptionUtility, IOptions<Configs> options)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

        var user = await applicationDbContext.Users.SingleOrDefaultAsync(q => q.UserName == model.UserName);
        if (user == null) throw new Exception();

        var hashPassowrd = encryptionUtility.GetSHA256(model.Password, user.PasswordSalt);
        if (user.Password != hashPassowrd) throw new Exception();

        var token = encryptionUtility.GetNewToken(user.Id);
        var refreshToken = encryptionUtility.GetNewRefreshToken();

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
            userRefreshToken.RefreshTokenTimeout = 15;
            userRefreshToken.CreateDate = DateTime.UtcNow;
            userRefreshToken.IsValid = true;
        }

        // Save changes to the database
        await unitOfWork.SaveChangesAsync();

        var result = new LoginResponseDTO
        {
            UserName = user.UserName,
            Token = token,
            RefreshToken = refreshToken
        };

        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        response.Result = result;
        return Results.Ok(response);
    }


}
