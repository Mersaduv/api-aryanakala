namespace ApiAryanakala.Entities;

public class ProductImage
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ThumbnailFileName { get; set; }
    public Product Products { get; set; }

}