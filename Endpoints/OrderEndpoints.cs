using ApiAryanakala.Const;
using ApiAryanakala.Entities;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiAryanakala.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderApi(this IEndpointRouteBuilder apiGroup)
    {
        var orderGroup = apiGroup.MapGroup(Constants.Order);

        orderGroup.MapGet(string.Empty, GetOrders);
        orderGroup.MapGet("/{orderId}", GetOrderDetails);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<List<OrderOverviewResponse>>>> GetOrders(
     IOrderServices orderService)
    {
        var response = await orderService.GetOrders();
        return TypedResults.Ok(response);
    }

    private static async Task<Ok<ServiceResponse<OrderDetailsResponse>>> GetOrderDetails(
        IOrderServices orderService, int orderId)
    {
        var response = await orderService.GetOrderDetails(orderId);
        return TypedResults.Ok(response);
    }
}