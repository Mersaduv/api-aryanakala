using ApiAryanakala.Models;

namespace ApiAryanakala.Interfaces.IServices;

public interface IPaymentService
{
    Session CreateCheckoutSession();

    ServiceResponse<bool> FullfillOrder(HttpRequest request);
}

public class Session
{
    //!class Stripe.Checkout.Session
    //? A Checkout Session represents your customer's session as they pay for one-time purchases or subscriptions through Checkout or Payment Links. We recommend creating a new Session each time your customer attempts to pay.
}