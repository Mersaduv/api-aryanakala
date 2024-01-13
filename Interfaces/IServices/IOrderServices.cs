using ApiAryanakala.Entities.Product;
using ApiAryanakala.Models;

namespace ApiAryanakala.Interfaces.IServices;

public interface IOrderServices
{
    Task<ServiceResponse<bool>> PlaceOrder(Guid userId);
    Task<ServiceResponse<List<OrderOverviewResponse>>> GetOrders();
    Task<ServiceResponse<OrderDetailsResponse>> GetOrderDetails(int orderId);
}