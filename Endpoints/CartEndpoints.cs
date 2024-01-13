using ApiAryanakala.Const;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiAryanakala.Endpoints;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartApi(this IEndpointRouteBuilder apiGroup)
    {
        var cartGroup = apiGroup.MapGroup(Constants.Cart);

        cartGroup.MapPost(string.Empty, StoreCartItems);

        cartGroup.MapPost(Constants.Add, AddToCart);

        cartGroup.MapPut(Constants.UpdateQuantity, UpdateQuantity);

        cartGroup.MapDelete("{productId}", RemoveItemFromCart);

        cartGroup.MapGet(Constants.Count, GetCartItemsCount);

        cartGroup.MapGet(string.Empty, GetDbCartProducts);

        return apiGroup;
    }

    private async static Task<Ok<ServiceResponse<List<CartProductResponse>>>> GetDbCartProducts(ICartService _cartService)
    {
        var results = await _cartService.GetDbCartProducts();
        return TypedResults.Ok(results);
    }

    private static async Task<Ok<ServiceResponse<List<CartProductResponse>>>> StoreCartItems(ICartService cartService,
            List<CartItem> cartItems)
    {
        var results = await cartService.StoreCartItems(cartItems);
        return TypedResults.Ok(results);
    }

       private static async Task<ServiceResponse<int>> GetCartItemsCount(ICartService cartService) =>
        await cartService.GetCartItemsCount();

    private static async Task<Ok<ServiceResponse<bool>>> AddToCart(ICartService cartService, [AsParameters] CartItem cartItem)
    {
        var results = await cartService.AddToCart(cartItem);
        return TypedResults.Ok(results);
    }

    private static async Task<Ok<ServiceResponse<bool>>> UpdateQuantity(ICartService cartService,
        CartItem cartItem)
    {
        var results = await cartService.UpdateQuantity(cartItem);
        return TypedResults.Ok(results);
    }

    private static async Task<Ok<ServiceResponse<bool>>> RemoveItemFromCart(ICartService cartService,
        Guid productId)
    {
        var results = await cartService.RemoveItemFromCart(productId);
        return TypedResults.Ok(results);
    }
}
