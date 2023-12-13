using System.Net;
using ApiAryanakala.Entities;
using ApiAryanakala.Filter;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Models;

namespace ApiAryanakala.Endpoints;

public static class BrandEndpoints
{
    public static void ConfigureBrandEndpoints(this WebApplication app)
    {
        app.MapPost("/api/brand", CreateBrand)
            .Produces(401)
            .Produces<APIResponse>(201)
            .Produces(400)
            .AddEndpointFilter<ModelValidationFilter<Brand>>()
            .ProducesValidationProblem()
            .WithName("CreateBrand")
            .Accepts<Brand>("application/json");

        app.MapPut("/api/brand", UpdateBrand)
            .WithName("UpdateBrand")
            .AddEndpointFilter<GuidValidationFilter>()
            .AddEndpointFilter<ModelValidationFilter<Brand>>()
            .ProducesValidationProblem()
            .Produces<APIResponse>(200)
            .Produces(400)
            .Accepts<Brand>("application/json");

        app.MapGet("/api/brand/{id:int}", GetBrand)
            .WithName("GetBrand").AddEndpointFilter<GuidValidationFilter>()
            .Produces<APIResponse>(200);

        app.MapGet("/api/brands", GetAllBrand)
            .WithName("GetBrands").Produces<APIResponse>(200);

        app.MapDelete("/api/brand/{id:int}", DeleteBrand)
            // .RequireAuthorization()
            .AddEndpointFilter<GuidValidationFilter>()
            .ProducesValidationProblem()
            .Produces(204).Produces(400);
    }

    private static async Task<IResult> CreateBrand(IGenericRepository<Brand> _brandRepository,
               Brand brand, ILogger<Program> _logger, HttpContext context)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
        _logger.Log(LogLevel.Information, "Create Brand");

        if (_brandRepository.GetByIdAsync(brand.Id).GetAwaiter().GetResult() != null)
        {
            response.ErrorMessages.Add("Brand Name/Id already Exists");
            return Results.BadRequest(response);
        }

        var result = await _brandRepository.Add(brand);

        response.Result = result;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.Created;
        return Results.Ok(response);
    }

    private async static Task<IResult> UpdateBrand(IGenericRepository<Brand> _brandRepository, Brand brand, ILogger<Program> _logger)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
        _logger.Log(LogLevel.Information, "Update Brand");

        var result = await _brandRepository.Update(brand);
        response.Result = result;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }



    private async static Task<IResult> DeleteBrand(IGenericRepository<Brand> _brandRepository, int id, ILogger<Program> _logger, HttpContext context)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
        _logger.Log(LogLevel.Information, "Delete Brand");

        var result = await _brandRepository.Delete(id);
        if (result > 0)
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

    private async static Task<IResult> GetBrand(IGenericRepository<Brand> _brandRepository, ILogger<Program> _logger, int id)
    {
        APIResponse response = new();
        _logger.Log(LogLevel.Information, "Get Brand");

        var result = await _brandRepository.GetByIdAsync(id);
        response.Result = result;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }

    private async static Task<IResult> GetAllBrand(IGenericRepository<Brand> _brandRepository, ILogger<Program> _logger)
    {
        APIResponse response = new();
        _logger.Log(LogLevel.Information, "Getting all Brands");

        var result = await _brandRepository.GetAllAsync();
        response.Result = result;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }

}
