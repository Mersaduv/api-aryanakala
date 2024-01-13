namespace ApiAryanakala.Entities.Product;

public class OrderDetailsProductResponse
{
    public Guid ProductId { get; set; }
    public string Title { get; set; }
    public string CategoryName { get; set; }
    public List<string> ImageUrl { get; set; }
    public int Quantity { get; set; }
    public double TotalPrice { get; set; }
}