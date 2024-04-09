using ApiAryanakala.Const;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiAryanakala.Endpoints;

public static class BannerEndpoints
{
    public static IEndpointRouteBuilder MapBannerApi(this IEndpointRouteBuilder apiGroup)
    {
        var bannerGroup = apiGroup.MapGroup(Constants.Banner);

        var bannersGroup = apiGroup.MapGroup(Constants.Banners);

        bannerGroup.MapPost(string.Empty, CreateBanner)
                   .Accepts<BannerCreateDto>("multipart/form-data");

        bannerGroup.MapPut(string.Empty, UpdateBanner);

        bannerGroup.MapGet("{id:guid}", GetBanner);

        bannersGroup.MapGet("{categoryId}", GetBannersByCategory);

        bannersGroup.MapGet(string.Empty, GetAllBanners);

        bannerGroup.MapDelete("{id:guid}", DeleteBanner);

        bannerGroup.MapPost($"/{Constants.Images}/{{id:guid}}", UpsertBannerImage)
                   .Accepts<Thumbnails>("multipart/form-data");

        bannerGroup.MapDelete($"/{Constants.Images}/{{fileName}}", DeleteBannerImage);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<bool>>> DeleteBannerImage(string fileName, IBannerServices bannerServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Delete Banner Image");

        var result = await bannerServices.DeleteBannerImage(fileName);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> UpsertBannerImage(Guid id, Thumbnails thumbnails, IBannerServices bannerServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Upsert Banner Images");

        var result = await bannerServices.UpsertBannerImage(thumbnails, id);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<BannerDto>>> CreateBanner(IBannerServices bannerServices,
               BannerCreateDto banner, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Banner");

        var result = await bannerServices.AddBanner(banner);
        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<BannerDto>>> UpdateBanner(IBannerServices bannerServices, BannerUpdateDto banner, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Banner");

        var result = await bannerServices.UpdateBanner(banner);
        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteBanner(IBannerServices bannerServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Banner");

        var result = await bannerServices.DeleteBanner(id);
        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<Banner>>> GetBanner(IBannerServices bannerServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Banner");

        var result = await bannerServices.GetBannerBy(id);
        return TypedResults.Ok(result);
    }
    private async static Task<Ok<ServiceResponse<IReadOnlyList<Banner>>>> GetBannersByCategory([FromQuery] int categoryId, IBannerServices bannerServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting Banners By Category Id");

        var result = await bannerServices.GetBannersByCategory(categoryId);
        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IReadOnlyList<Banner>>>> GetAllBanners(IBannerServices bannerServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all Banners");

        var result = await bannerServices.GetBanners();
        return TypedResults.Ok(result);
    }
}
