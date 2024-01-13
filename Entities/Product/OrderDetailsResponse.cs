namespace ApiAryanakala.Entities.Product;

public class OrderDetailsResponse
{
    public DateTime OrderDate { get; set; }
    public double TotalPrice { get; set; }
    public List<OrderDetailsProductResponse> Products { get; set; }
}