using System.Collections.Generic;
namespace ApiAryanakala.Models.DTO.ProductDtos;

public class ProductDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public List<string>? ImagesSrc { get; set; }
    public string Code { get; set; }
    public string Slug { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public double? Discount { get; set; }
    public List<string> Category { get; set; }
    public int CategoryId { get; set; }
    public List<string> Size { get; set; }
    // public List<string> Colors { get; set; }
    public List<InfoDto> Info { get; set; }
    public int InStock { get; set; }
    public int? Sold { get; set; }
    public double Rating { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastUpdated { get; set; }
}