using ApiAryanakala.Const;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiAryanakala.Endpoints;

public static class BrandEndpoints
{
    public static IEndpointRouteBuilder MapBrandApi(this IEndpointRouteBuilder apiGroup)
    {
        var brandGroup = apiGroup.MapGroup(Constants.Brand);

        apiGroup.MapGet(Constants.Brands, GetAllBrand);

        brandGroup.MapPost(string.Empty, CreateBrand);

        brandGroup.MapPut(string.Empty, UpdateBrand);

        brandGroup.MapGet("{id:int}", GetBrand);

        brandGroup.MapDelete("{id:int}", DeleteBrand);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<int>>> CreateBrand(IProductServices productServices,
               Brand brand, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Brand");

        var result = await productServices.AddBrand(brand);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<int>>> UpdateBrand(IProductServices productServices, Brand brand, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Brand");

        var result = await productServices.UpdateBrand(brand);

        return TypedResults.Ok(result);

    }



    private async static Task<Ok<ServiceResponse<bool>>> DeleteBrand(IProductServices productServices, int id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Brand");

        var result = await productServices.DeleteBrand(id);

        return TypedResults.Ok(result);

    }

    private async static Task<Ok<ServiceResponse<Brand>>> GetBrand(IProductServices productServices, ILogger<Program> _logger, int id)
    {
        _logger.Log(LogLevel.Information, "Get Brand");

        var result = await productServices.GetBrandBy(id);

        return TypedResults.Ok(result);

    }

    private async static Task<Ok<ServiceResponse<IReadOnlyList<Brand>>>> GetAllBrand(IProductServices productServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all Brands");

        var result = await productServices.GetBrands();

        return TypedResults.Ok(result);

    }

}
