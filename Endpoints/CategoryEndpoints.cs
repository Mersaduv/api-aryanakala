using ApiAryanakala.Const;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto;
using ApiAryanakala.Models.DTO.ProductDto.Category;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiAryanakala.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryApi(this IEndpointRouteBuilder apiGroup)
    {
        var categoryGroup = apiGroup.MapGroup(Constants.Category);

        apiGroup.MapGet(Constants.Categories, GetAllCategory);

        categoryGroup.MapPost(string.Empty, CreateCategory)
        .Accepts<CategoryCreateDTO>("multipart/form-data");

        categoryGroup.MapPut(string.Empty, UpdateCategory);

        categoryGroup.MapGet("{id:int}", GetCategory);

        categoryGroup.MapDelete("{id:int}", DeleteCategory);

        categoryGroup.MapPost($"/{Constants.CategoryImages}/{{id:int}}", UpsertCategoryImages)
        .Accepts<Thumbnails>("multipart/form-data");

        categoryGroup.MapDelete($"/{Constants.CategoryImages}/{{fileName}}", DeleteCategoryImage);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<CategoryDTO>>> CreateCategory(ICategoryService categoryServices,
               CategoryCreateDTO category, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Category");

        var result = await categoryServices.Add(category);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<CategoryDTO>>> UpdateCategory(ICategoryService categoryServices,
               CategoryUpdateDTO category, ILogger<Program> _logger)
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

    private async static Task<Ok<ServiceResponse<IEnumerable<CategoryDTO>>>> GetAllCategory(ICategoryService categoryServices, ILogger<Program> _logger)
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

    private static async Task<Ok<ServiceResponse<bool>>> DeleteCategoryImage(string fileName, ICategoryService categoryService, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Delete Product Images");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryService.DeleteCategoryImages(fileName);
        return TypedResults.Ok(result);
    }


    private static async Task<Ok<ServiceResponse<bool>>> UpsertCategoryImages(int id, Thumbnails thumbnails, ICategoryService categoryService, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Upsert Product Images");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryService.UpsertCategoryImages(thumbnails, id);
        return TypedResults.Ok(result);
    }


}