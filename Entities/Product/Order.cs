using ApiAryanakala.Models;

namespace ApiAryanakala.Entities.Product;

public class Order : BaseClass<int>
{
    public Guid UserId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public double TotalPrice { get; set; }
    public List<OrderItem> OrderItems { get; set; } = [];
}