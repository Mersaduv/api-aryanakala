using ApiAryanakala.Data;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Utility;
using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Services.Product;

public class OrderService : IOrderServices
{
    private readonly ApplicationDbContext _context;
    private readonly ICartService _cartService;
    private readonly IAuthServices _authService;
    private readonly ByteFileUtility byteFileUtility;


    public OrderService(ApplicationDbContext context,
        ICartService cartService,
        IAuthServices authService,
        ByteFileUtility byteFileUtility)
    {
        _context = context;
        _cartService = cartService;
        _authService = authService;
        this.byteFileUtility = byteFileUtility;

    }

    public async Task<ServiceResponse<OrderDetailsResponse>> GetOrderDetails(int orderId)
    {
        var response = new ServiceResponse<OrderDetailsResponse>();
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Category)
            .Where(o => o.UserId == _authService.GetUserId() && o.Id == orderId)
            .OrderByDescending(o => o.OrderDate)
            .FirstOrDefaultAsync();

        if (order == null)
        {
            response.Success = false;
            response.Message = "Order not found.";
            return response;
        }

        var orderDetailsResponse = new OrderDetailsResponse
        {
            OrderDate = order.OrderDate,
            TotalPrice = order.TotalPrice,
            Products = new List<OrderDetailsProductResponse>()
        };

        order.OrderItems.ForEach(item =>
        orderDetailsResponse.Products.Add(new OrderDetailsProductResponse
        {
            ProductId = item.ProductId,
            ImageUrl = byteFileUtility.GetEncryptedFileActionUrl(item.Product.Images.Select(img => img.ThumbnailFileName).ToList(), nameof(Product)),
            CategoryName = item.Category.Name,
            Quantity = item.Quantity,
            Title = item.Product.Title,
            TotalPrice = item.TotalPrice
        }));

        response.Data = orderDetailsResponse;

        return response;
    }

    public async Task<ServiceResponse<List<OrderOverviewResponse>>> GetOrders()
    {
        var response = new ServiceResponse<List<OrderOverviewResponse>>();
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == _authService.GetUserId())
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var orderResponse = new List<OrderOverviewResponse>();
        orders.ForEach(o => orderResponse.Add(new OrderOverviewResponse
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            TotalPrice = o.TotalPrice,
            Product = o.OrderItems.Count > 1 ?
                $"{o.OrderItems.First().Product.Title} and" +
                $" {o.OrderItems.Count - 1} more..." :
                o.OrderItems.First().Product.Title,
            ProductImageUrl = byteFileUtility.GetEncryptedFileActionUrl(o.OrderItems.First().Product.Images.Select(img => img.ThumbnailFileName).ToList(), nameof(Product))
        }));

        response.Data = orderResponse;

        return response;
    }

    public async Task<ServiceResponse<bool>> PlaceOrder(Guid userId)
    {
        var products = (await _cartService.GetDbCartProducts(userId)).Data;
        double totalPrice = 0;
        products.ForEach(product => totalPrice += product.Price * product.Quantity);

        var orderItems = new List<OrderItem>();
        products.ForEach(product => orderItems.Add(new OrderItem
        {
            ProductId = product.ProductId,
            CategoryId = product.CategoryId,
            Quantity = product.Quantity,
            TotalPrice = product.Price * product.Quantity
        }));

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            TotalPrice = totalPrice,
            OrderItems = orderItems
        };

        _context.Orders.Add(order);

        _context.CartItems.RemoveRange(_context.CartItems
            .Where(ci => ci.UserId == userId));

        await _context.SaveChangesAsync();

        return new ServiceResponse<bool> { Data = true };
    }
}