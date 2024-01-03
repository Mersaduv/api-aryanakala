using ApiAryanakala.Const;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiAryanakala.Endpoints;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentApi(this IEndpointRouteBuilder apiGroup)
    {
        var paymentGroup = apiGroup.MapGroup(Constants.Payment);

        paymentGroup.MapPost(Constants.Checkout, CreateCheckoutSessionAsync);
        paymentGroup.MapPost(string.Empty, FullfillOrderAsync);

        return apiGroup;
    }
    private static async Task<Ok<string>> CreateCheckoutSessionAsync(IPaymentService paymentService)
    {
        var session = await paymentService.CreateCheckoutSession();
        return TypedResults.Ok(string.Empty);
    }

    private static async Task<Results<Ok<ServiceResponse<bool>>, BadRequest<string>>> FullfillOrderAsync(
        IPaymentService paymentService,
        HttpRequest request)
    {
        var response = await paymentService.FullfillOrderAsync(request);
        return !response.Success
            ? TypedResults.BadRequest(response.Message)
            : TypedResults.Ok(response);
    }
}