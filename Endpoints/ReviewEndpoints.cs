using System.Net;
using ApiAryanakala.Const;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto.Review;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiAryanakala.Endpoints;

public static class ReviewEndpoints
{
    public static IEndpointRouteBuilder MapReviewApi(this IEndpointRouteBuilder apiGroup)
    {
        var reviewGroup = apiGroup.MapGroup(Constants.Review);
        apiGroup.MapGet(Constants.Reviews, GetReviews);

        reviewGroup.MapPost(string.Empty, CreateReview);
        reviewGroup.MapGet("/{id:guid}", GetProductReviews);
        reviewGroup.MapDelete("/{id:guid}", DeleteReview);
        reviewGroup.MapGet("{id:guid}", GetReview);

        return apiGroup;
    }

    private async static Task<Ok<ServiceResponse<ReviewDto>>> GetReview(IReviewServices reviewServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Get Single Review");

        var result = await reviewServices.GetReviewBy(id);

        return TypedResults.Ok(result);
    }


    private async static Task<Ok<ServiceResponse<bool>>> DeleteReview(IReviewServices reviewServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Review");

        var result = await reviewServices.DeleteReview(id);

        return TypedResults.Ok(result);
    }


    private async static Task<Ok<ServiceResponse<PagingModel<ReviewDto>>>> GetReviews(IReviewServices reviewServices, [FromQuery] int page, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Get Reviews");

        var result = await reviewServices.GetReviews(page);

        return TypedResults.Ok(result);
    }


    private async static Task<Ok<ServiceResponse<bool>>> CreateReview(IReviewServices reviewServices, ReviewCreateDTO command, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Review");

        var result = await reviewServices.CreateReview(command);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<PagingModel<ReviewDto>>>> GetProductReviews(IReviewServices reviewServices, [AsParameters] GetReviewDto query, Guid id, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Product Reviews");

        var result = await reviewServices.GetProductReviews(new GetReviewQuery(id, query));

        return TypedResults.Ok(result);
    }
}
