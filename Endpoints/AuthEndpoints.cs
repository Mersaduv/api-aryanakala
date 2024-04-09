using System.Security.Claims;
using ApiAryanakala.Const;
using ApiAryanakala.Entities.User;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiAryanakala.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthApi(this IEndpointRouteBuilder apiGroup)
    {
        var authGroup = apiGroup.MapGroup(Constants.Auth);

        authGroup.MapPost(Constants.Register, RegisterUser);
        authGroup.MapPost(Constants.Login, LogInUser);
        authGroup.MapPost(Constants.ChangePassword, ChangePasswordAsync).RequireAuthorization();
        authGroup.MapPost(Constants.GenerateRefreshToken, GenerateNewToken);

        return apiGroup;
    }

    private static async Task<Results<Ok<ServiceResponse<GenerateNewTokenDTO>>, BadRequest<ServiceResponse<GenerateNewTokenDTO>>>> GenerateNewToken(
    IAuthServices authService, GenerateNewTokenDTO request)
    {
        var response = await authService.GenerateNewToken(request);
        return !response.Success ? TypedResults.BadRequest(response) : TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ServiceResponse<Guid>>, BadRequest<ServiceResponse<Guid>>>> RegisterUser(
        IAuthServices authService, UserRegister request)
    {
        var response = await authService.RegisterAsync(new User { Email = request.Email, UserName = request.UserName }, request.Password);
        return !response.Success ? TypedResults.BadRequest(response) : TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ServiceResponse<LoginResponse>>, BadRequest<ServiceResponse<LoginResponse>>>> LogInUser(
        IAuthServices authService, UserLogin request)
    {
        var response = await authService.LogInAsync(request.Email, request.Password);
        return !response.Success ? TypedResults.BadRequest(response) : TypedResults.Ok(response);
    }
    private static async Task<Results<Ok<ServiceResponse<bool>>, BadRequest<ServiceResponse<bool>>>>
        ChangePasswordAsync(IAuthServices authService, ClaimsPrincipal user, [FromBody] string newPassword)
    {
        var userId = authService.GetUserId();
        if (userId == Guid.Empty)
        {
            return TypedResults.BadRequest(new ServiceResponse<bool>());
        }

        var response = await authService.ChangePasswordAsync(userId, newPassword);
        return !response.Success ? TypedResults.BadRequest(response) : TypedResults.Ok(response);
    }
}

