using System.ComponentModel.DataAnnotations;

namespace ApiAryanakala.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public List<ProductImage>? Images { get; set; }
    public string Code { get; set; }
    public string Slug { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public double? Discount { get; set; }
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; }
    public int BrandId { get; set; }
    public virtual Brand Brand { get; set; }
    public List<ProductColor>? Colors { get; set; }
    public List<string>? Size { get; set; }
    public List<ProductAttribute> ProductAttribute { get; set; }
    public int InStock { get; set; }
    public int? Sold { get; set; }
    public double? Rating { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastUpdated { get; set; }
}


public class ProductAttribute
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Title { get; set; }
    public string Value { get; set; }
    public Product Products { get; set; }
}

public class ProductColor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string HashCode { get; set; }
}