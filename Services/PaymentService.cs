using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using Microsoft.Extensions.Options;

namespace ApiAryanakala.Services;

public record PaymentService(
    ICartService CartService,
    IAuthServices AuthService,
    IOrderServices OrderService,
    IOptions<Configs> Options) : IPaymentService
{
    private const string ClientBaseUrl = @"https://localhost:7177";

    public async Task<Session> CreateCheckoutSession()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<bool>> FullfillOrderAsync(HttpRequest request)
    {
        throw new NotImplementedException();

    }
}
