namespace ApiAryanakala.Entities.Product;

public class Order
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;

    public double TotalPrice { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}