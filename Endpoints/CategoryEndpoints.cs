using System.Net;
using ApiAryanakala.Const;
using ApiAryanakala.Entities;
using ApiAryanakala.Filter;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDtos.Category;
using Microsoft.AspNetCore.Http.HttpResults;
using X.PagedList;

namespace ApiAryanakala.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryApi(this IEndpointRouteBuilder apiGroup)
    {
        var categoryGroup = apiGroup.MapGroup(Constants.Category);

        apiGroup.MapGet(Constants.Categories, GetAllCategory);

        categoryGroup.MapPost(string.Empty, CreateCategory);

        categoryGroup.MapPut(string.Empty, UpdateCategory);

        categoryGroup.MapGet("{id:int}", GetCategory);

        categoryGroup.MapDelete("{id:int}", DeleteCategory);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<CategoryDTO>>> CreateCategory(ICategoryService categoryServices,
               Category category, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Category");

        var result = await categoryServices.Add(category);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<CategoryDTO>>> UpdateCategory(ICategoryService categoryServices,
               Category category, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Category");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryServices.Update(category);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<CategoryDTO>>> GetCategory(ICategoryService categoryServices,
     ILogger<Program> _logger, int id)
    {
        _logger.Log(LogLevel.Information, "Get Category");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryServices.GetBy(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IPagedList<CategoryDTO>>>> GetAllCategory(ICategoryService categoryServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all Categories");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryServices.GetAllCategories(null, 10);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteCategory(ICategoryService categoryServices, int id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Category");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryServices.Delete(id);

        return TypedResults.Ok(result);
    }

}