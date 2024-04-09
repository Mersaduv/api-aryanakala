using ApiAryanakala.Models;

namespace ApiAryanakala.Entities.Product;

public class ProductColor
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string HashCode { get; set; } = string.Empty;
}