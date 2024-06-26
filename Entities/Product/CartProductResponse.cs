using ApiAryanakala.Models.DTO;

namespace ApiAryanakala.Entities.Product;

public class CartProductResponse
{
    public Guid ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<EntityImageDto>? ImageUrl { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
}