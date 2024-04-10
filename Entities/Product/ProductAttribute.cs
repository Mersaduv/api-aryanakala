using ApiAryanakala.Models;

namespace ApiAryanakala.Entities.Product;

public class ProductAttribute : BaseClass<Guid>
{
    public Guid ProductId { get; set; }
    public required string Title { get; set; }
    public string? Value { get; set; }
    public virtual Product? ProductsInfo { get; set; }
    public virtual Product? ProductsSpecification { get; set; }
}
