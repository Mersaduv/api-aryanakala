namespace ApiAryanakala.Entities.Product;

public class OrderOverviewResponse
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public double TotalPrice { get; set; }
    public string Product { get; set; }
    public List<string> ProductImageUrl { get; set; }
}