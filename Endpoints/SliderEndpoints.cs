using ApiAryanakala.Const;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto;
using Microsoft.AspNetCore.Http.HttpResults;

public static class SliderEndpoints
{
    public static IEndpointRouteBuilder MapSliderApi(this IEndpointRouteBuilder apiGroup)
    {
        var sliderGroup = apiGroup.MapGroup(Constants.Slider);

        apiGroup.MapGet(Constants.Sliders, GetAllSliders);

        sliderGroup.MapPost(string.Empty, CreateSlider)
        .Accepts<SliderCreateDto>("multipart/form-data");

        sliderGroup.MapPut(string.Empty, UpdateSlider);

        sliderGroup.MapGet("{id:guid}", GetSlider);

        sliderGroup.MapDelete("{id:guid}", DeleteSlider);

        sliderGroup.MapPost($"/{Constants.CategoryImages}/{{id:guid}}", UpsertSliderCategoryImages)
        .Accepts<Thumbnails>("multipart/form-data");

        sliderGroup.MapDelete($"/{Constants.CategoryImages}/{{fileName}}", DeleteSliderCategoryImage);

        return apiGroup;
    }
    private static async Task<Ok<ServiceResponse<bool>>> DeleteSliderCategoryImage(string fileName, ISliderServices sliderServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Delete Slider Images");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await sliderServices.DeleteSliderImages(fileName);
        return TypedResults.Ok(result);
    }


    private static async Task<Ok<ServiceResponse<bool>>> UpsertSliderCategoryImages(Guid id, Thumbnails thumbnails, ISliderServices sliderServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Upsert Slider Images");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await sliderServices.UpsertSliderImages(thumbnails, id);
        return TypedResults.Ok(result);
    }
    private static async Task<Ok<ServiceResponse<SliderDto>>> CreateSlider(ISliderServices sliderServices,
               SliderCreateDto slider, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Slider");

        var result = await sliderServices.AddSlider(slider);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<SliderDto>>> UpdateSlider(ISliderServices sliderServices, SliderUpdateDto slider, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Slider");

        var result = await sliderServices.UpdateSlider(slider);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteSlider(ISliderServices sliderServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Slider");

        var result = await sliderServices.DeleteSlider(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<Slider>>> GetSlider(ISliderServices sliderServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Slider");

        var result = await sliderServices.GetSliderBy(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IReadOnlyList<Slider>>>> GetAllSliders(ISliderServices sliderServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all Sliders");

        var result = await sliderServices.GetSliders();

        return TypedResults.Ok(result);
    }
}
