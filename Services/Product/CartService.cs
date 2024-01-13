using ApiAryanakala.Data;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Utility;
using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Services.Product;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthServices _authService;
    private readonly ByteFileUtility _byteFileUtility;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryService _categoryService;

    public CartService(ApplicationDbContext context,
     IAuthServices authService,
     ByteFileUtility byteFileUtility,
     IProductRepository productRepository,
     ICategoryService categoryService)
    {
        _context = context;
        _authService = authService;
        _byteFileUtility = byteFileUtility;
        _productRepository = productRepository;
        _categoryService = categoryService;

    }

    public async Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems)
    {
        var result = new ServiceResponse<List<CartProductResponse>>
        {
            Data = new List<CartProductResponse>()
        };

        foreach (var item in cartItems)
        {
            var product = await _productRepository.GetAsyncBy(item.ProductId);
            if (product == null)
            {
                continue;
            }

            var category = await _categoryService.GetCategoryAsyncBy(item.CategoryId);
            if (category == null)
            {
                continue;
            }

            var cartProduct = new CartProductResponse
            {
                ProductId = product.Id,
                Title = product.Title,
                ImageUrl = _byteFileUtility.GetEncryptedFileActionUrl(product.Images.Select(img => img.ThumbnailFileName).ToList(), nameof(Product)),
                Price = product.Price,
                CategoryName = category.Name,
                CategoryId = category.Id,
                Quantity = item.Quantity
            };

            result.Data.Add(cartProduct);
        }

        return result;
    }

    public async Task<ServiceResponse<List<CartProductResponse>>> StoreCartItems(List<CartItem> cartItems)
    {
        cartItems.ForEach(cartItem => cartItem.UserId = _authService.GetUserId());
        _context.CartItems.AddRange(cartItems);
        await _context.SaveChangesAsync();

        return await GetDbCartProducts();
    }

    public async Task<ServiceResponse<int>> GetCartItemsCount()
    {
        var count = (await _context.CartItems.Where(ci => ci.UserId == _authService.GetUserId()).ToListAsync()).Count;
        return new ServiceResponse<int> { Data = count };
    }

    public async Task<ServiceResponse<List<CartProductResponse>>> GetDbCartProducts(Guid? userId = null)
    {
        if (userId == null)
            userId = _authService.GetUserId();

        return await GetCartProducts(await _context.CartItems
            .Where(ci => ci.UserId == userId).ToListAsync());
    }

    public async Task<ServiceResponse<bool>> AddToCart(CartItem cartItem)
    {
        cartItem.UserId = _authService.GetUserId();

        var sameItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId &&
            ci.CategoryId == cartItem.CategoryId && ci.UserId == cartItem.UserId);
        if (sameItem == null)
        {
            _context.CartItems.Add(cartItem);
        }
        else
        {
            sameItem.Quantity += cartItem.Quantity;
        }

        await _context.SaveChangesAsync();

        return new ServiceResponse<bool> { Data = true };
    }

    public async Task<ServiceResponse<bool>> UpdateQuantity(CartItem cartItem)
    {
        var dbCartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId &&
            ci.CategoryId == cartItem.CategoryId && ci.UserId == _authService.GetUserId());
        if (dbCartItem == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Success = false,
                Message = "Cart item does not exist."
            };
        }

        dbCartItem.Quantity = cartItem.Quantity;
        await _context.SaveChangesAsync();

        return new ServiceResponse<bool> { Data = true };
    }

    public async Task<ServiceResponse<bool>> RemoveItemFromCart(Guid productId)
    {
        var dbCartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.ProductId == productId &&
             ci.UserId == _authService.GetUserId());
        if (dbCartItem == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Success = false,
                Message = "Cart item does not exist."
            };
        }

        _context.CartItems.Remove(dbCartItem);
        await _context.SaveChangesAsync();

        return new ServiceResponse<bool> { Data = true };
    }
}