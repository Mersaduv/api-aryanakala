using System.Net;
using ApiAryanakala.Entities;
using ApiAryanakala.Filter;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;

namespace ApiAryanakala.Endpoints;

public static class CategoryEndpoints
{
    public static void ConfigureCategoryEndpoints(this WebApplication app)
    {
        app.MapPost("/api/category", CreateCategory)
        .Produces(401)
        .Produces<APIResponse>(201)
        .Produces(400)
        // .AddEndpointFilter<ModelValidationFilter<Category>>()
        .ProducesValidationProblem()
        .WithName("CreateCategory")
        .Accepts<Category>("application/json");


        app.MapPut("/api/category", UpdateCategory)
        .WithName("UpdateCategory")
        .AddEndpointFilter<GuidValidationFilter>()
        .AddEndpointFilter<ModelValidationFilter<Category>>()
        .ProducesValidationProblem()
        .Produces<APIResponse>(200)
        .Produces(400)
        .Accepts<Category>("application/json");

        app.MapGet("/api/category/{id:int}", GetCategory)
        .WithName("GetCategory").AddEndpointFilter<GuidValidationFilter>()
        .Produces<APIResponse>(200);

        app.MapGet("/api/categories", GetAllCategory)
        .WithName("GetCategories").Produces<APIResponse>(200);

        app.MapDelete("/api/category/{id:int}", DeleteCategory)
        // .RequireAuthorization()
        .AddEndpointFilter<GuidValidationFilter>()
        .ProducesValidationProblem()
        .Produces(204).Produces(400);
    }

    private static async Task<IResult> CreateCategory(ICategoryService categoryServices,
               Category category, ILogger<Program> _logger, HttpContext context)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
        _logger.Log(LogLevel.Information, "Create Category");


        if (categoryServices.GetBy(category.Id).GetAwaiter().GetResult() != null)
        {
            response.ErrorMessages.Add("Category Name/Id already Exists");
            return Results.BadRequest(response);
        }

        var result = await categoryServices.Add(category);

        response.Result = result;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.Created;
        return Results.Ok(response);
    }

    private async static Task<IResult> UpdateCategory(ICategoryService categoryServices,
               Category category, ILogger<Program> _logger)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
        _logger.Log(LogLevel.Information, "Update Category");


        var result = await categoryServices.Update(category);
        response.Result = result;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }

    private async static Task<IResult> GetCategory(ICategoryService categoryServices,
     ILogger<Program> _logger, int id)
    {
        APIResponse response = new();
        _logger.Log(LogLevel.Information, "Get Category");

        var result = await categoryServices.GetBy(id);
        response.Result = result;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }

    private async static Task<IResult> GetAllCategory(ICategoryService categoryServices, ILogger<Program> _logger)
    {
        APIResponse response = new();
        _logger.Log(LogLevel.Information, "Getting all Categories");


        var result = await categoryServices.GetAll();
        response.Result = result;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }

    private async static Task<IResult> DeleteCategory(ICategoryService categoryServices, int id, ILogger<Program> _logger, HttpContext context)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
        _logger.Log(LogLevel.Information, "Delete Category");

        var result = await categoryServices.Delete(id);
        if (result)
        {
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.NoContent;
            return Results.Ok(response);
        }
        else
        {
            response.ErrorMessages.Add("Invalid Id");
            return Results.BadRequest(response);
        }
    }

}