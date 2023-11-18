using System.ComponentModel.DataAnnotations;

namespace ApiAryanakala.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public List<ProductImage> Images { get; set; }
    public string Code { get; set; }
    public string Slug { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public double? Discount { get; set; }
    public List<string> Category { get; set; }
    public List<string> Size { get; set; }
    public List<Info> Info { get; set; }
    public long InfoId { get; internal set; }
    public int InStock { get; set; }
    public int? Sold { get; set; }
    public double Rating { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastUpdated { get; set; }
}


public class Info
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Title { get; set; }
    public string Value { get; set; }
    public Product Products { get; set; }
}
