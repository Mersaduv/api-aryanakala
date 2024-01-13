namespace ApiAryanakala.Entities.Product;

public class OrderItem
{
    public Order Order { get; set; }
    public int OrderId { get; set; }
    public Product Product { get; set; }
    public Guid ProductId { get; set; }
    public Category Category { get; set; }
    public int CategoryId { get; set; }
    public int Quantity { get; set; }
    public double TotalPrice { get; set; }
}
