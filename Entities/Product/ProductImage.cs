using ApiAryanakala.Interfaces;

namespace ApiAryanakala.Entities.Product;

public class ProductImage : IThumbnail
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ThumbnailFileName { get; set; }
    public Product Products { get; set; }

}