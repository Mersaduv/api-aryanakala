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

        paymentGroup.MapPost(Constants.Checkout, CreateCheckoutSession);
        paymentGroup.MapPost(string.Empty, FullfillOrder);

        return apiGroup;
    }
    private static Ok<string> CreateCheckoutSession(IPaymentService paymentService)
    {
        var session = paymentService.CreateCheckoutSession();
        return TypedResults.Ok(string.Empty);
    }

    private static Results<Ok<ServiceResponse<bool>>, BadRequest<string>> FullfillOrder(
        IPaymentService paymentService,
        HttpRequest request)
    {
        var response = paymentService.FullfillOrder(request);
        return !response.Success
            ? TypedResults.BadRequest(response.Message)
            : TypedResults.Ok(response);
    }
}