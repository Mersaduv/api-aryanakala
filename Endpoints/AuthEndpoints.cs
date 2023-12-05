using System.Net;
using ApiAryanakala.Data;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO;
using ApiAryanakala.Utility;
using Microsoft.AspNetCore.Mvc;
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
         IUnitOfWork unitOfWork, IOptions<Configs> options, IAuthServices authService)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            var result = await authService.GenerateNewToken(model);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = result;
            return Results.Ok(response);
        }


        private async static Task<IResult> Register(IUnitOfWork unitOfWork, LoginRequestDTO model, EncryptionUtility encryptionUtility, ApplicationDbContext applicationDbContext,
        IAuthServices authServices)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            await authServices.Register(model);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }


        private async static Task<IResult> Login(IUnitOfWork unitOfWork, ApplicationDbContext applicationDbContext,
            [FromBody] LoginRequestDTO model, EncryptionUtility encryptionUtility, IOptions<Configs> options, IAuthServices authService)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            var result = await authService.Login(model);
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = result;
            return Results.Ok(response);
        }


    }
}

