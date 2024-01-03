using System.Net;
using ApiAryanakala.Const;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDtos.Rating;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiAryanakala.Endpoints;

public static class RatingEndpoints
{
    public static IEndpointRouteBuilder MapRatingApi(this IEndpointRouteBuilder apiGroup)
    {
        var ratingGroup = apiGroup.MapGroup(Constants.Rating);

        ratingGroup.MapPost(string.Empty, CreateRating);
        ratingGroup.MapGet("{slug}", GetRatings);

        return apiGroup;
    }

    private async static Task<Ok<ServiceResponse<bool>>> CreateRating(IRatingServices ratingServices, RatingCreateDTO command, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Rating");

        var dto = await ratingServices.CreateRating(command);

        return TypedResults.Ok(dto);
    }

    private async static Task<Ok<ServiceResponse<PagingModel<RatingDto>>>> GetRatings(IRatingServices ratingServices, [AsParameters] GetRatingDto query, string slug, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Ratings");

        var dto = await ratingServices.GetRating(new GetRatingQuery(slug, query));

        return TypedResults.Ok(dto);
    }
}
