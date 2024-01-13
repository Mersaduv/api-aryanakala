using ApiAryanakala.Entities.Product;
using ApiAryanakala.Models;

namespace ApiAryanakala.Interfaces.IServices;

public interface ICartService
{
    Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems);
    Task<ServiceResponse<List<CartProductResponse>>> StoreCartItems(List<CartItem> cartItems);
    Task<ServiceResponse<int>> GetCartItemsCount();
    Task<ServiceResponse<List<CartProductResponse>>> GetDbCartProducts(Guid? userId = null);
    Task<ServiceResponse<bool>> AddToCart(CartItem cartItem);
    Task<ServiceResponse<bool>> UpdateQuantity(CartItem cartItem);
    Task<ServiceResponse<bool>> RemoveItemFromCart(Guid productId);
}