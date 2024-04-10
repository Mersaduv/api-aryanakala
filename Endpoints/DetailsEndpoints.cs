using ApiAryanakala.Const;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiAryanakala.Endpoints;

public static class DetailsEndpoints
{
    public static IEndpointRouteBuilder MapDetailsApi(this IEndpointRouteBuilder apiGroup)
    {
        var detailsGroup = apiGroup.MapGroup(Constants.Details);

        apiGroup.MapGet(Constants.AllDetails, GetAllDetails);

        detailsGroup.MapPost(string.Empty, CreateDetails);

        detailsGroup.MapPut(string.Empty, UpdateDetails);

        detailsGroup.MapGet("single-details/{id:guid}", GetDetails);

        detailsGroup.MapDelete("{id:guid}", DeleteDetails);

        detailsGroup.MapGet("{id:int}", GetDetailsByCategory);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<Details>>> CreateDetails(IDetailsServices detailsService, Details details, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Details");

        var result = await detailsService.AddDetails(details);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<Guid?>>> UpdateDetails(IDetailsServices detailsService, Details details, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Details");

        var result = await detailsService.UpdateDetails(details);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteDetails(IDetailsServices detailsService, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Details");

        var result = await detailsService.DeleteDetails(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<Details>>> GetDetails(IDetailsServices detailsService, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Details");

        var result = await detailsService.GetDetailsBy(id);

        return TypedResults.Ok(result);
    }
    private async static Task<Ok<ServiceResponse<Details>>> GetDetailsByCategory(IDetailsServices detailsService, ILogger<Program> _logger, int id)
    {
        _logger.Log(LogLevel.Information, "Get Details");

        var result = await detailsService.GetDetailsByCategory(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IReadOnlyList<Details>>>> GetAllDetails(IDetailsServices detailsService, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all Details");

        var result = await detailsService.GetAllDetails();

        return TypedResults.Ok(result);
    }
}
