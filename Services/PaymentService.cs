using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using Microsoft.Extensions.Options;

namespace ApiAryanakala.Services;

public record PaymentService(
    ICartService CartService,
    IAuthServices AuthService,
    IOrderServices OrderService,
    AuthSettings AuthSettings) : IPaymentService
{
    private const string ClientBaseUrl = @"https://localhost:7177";

    public Session CreateCheckoutSession()
    {
        throw new NotImplementedException();
    }

    public ServiceResponse<bool> FullfillOrder(HttpRequest request)
    {
        throw new NotImplementedException();

    }
}
