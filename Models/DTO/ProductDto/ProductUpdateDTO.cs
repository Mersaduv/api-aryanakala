using ApiAryanakala.Entities.Product;

namespace ApiAryanakala.Models.DTO.ProductDto;

public class ProductUpdateDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public double Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public double? Discount { get; set; }
    public int CategoryId { get; set; }
    public int? BrandId { get; set; }
    public List<string>? Size { get; set; }
    public List<ProductColor>? Colors { get; set; }
    public List<ProductAttributeDto>? Info { get; set; }
    public List<ProductAttributeDto>? Specification { get; set; }
    public int InStock { get; set; }
    public int? Sold { get; set; }
    // public int? Rating { get; set; }
    // public int? NumReviews { get; set; }
}
