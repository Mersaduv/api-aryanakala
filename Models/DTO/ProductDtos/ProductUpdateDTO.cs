using System.Reflection;
using ApiAryanakala.Entities;
using ApiAryanakala.Utility;
using Newtonsoft.Json;

namespace ApiAryanakala.Models.DTO.ProductDtos;

public class ProductUpdateDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Code { get; set; }
    public string Slug { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public double? Discount { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public List<string>? Size { get; set; }
    public List<ProductColor>? Colors { get; set; }
    public List<ProductAttributeDto> ProductAttribute { get; set; }
    public int InStock { get; set; }
    public int? Sold { get; set; }
    public double? Rating { get; set; }
}
