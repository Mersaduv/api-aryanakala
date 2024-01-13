using ApiAryanakala.Interfaces;

namespace ApiAryanakala.Entities.Product;

public class CategoryImage : IThumbnail
{
    public Guid Id { get; set; }
    public int CategoryId { get; set; }
    public string? ThumbnailFileName { get; set; }
    public Category Categories { get; set; }

}