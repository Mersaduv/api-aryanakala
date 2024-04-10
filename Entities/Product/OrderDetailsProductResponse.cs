using ApiAryanakala.Models.DTO;

namespace ApiAryanakala.Entities.Product;

public class OrderDetailsProductResponse
{
    public Guid ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public List<EntityImageDto> ImageUrl { get; set; } = default!;
    public int Quantity { get; set; }
    public double TotalPrice { get; set; }
}