using ApiAryanakala.Entities.Product;

namespace ApiAryanakala.Models;
public abstract class ProductAttributeBase : BaseClass<Guid>
{
    public Guid ProductId { get; set; }
    public required string Title { get; set; }
    public virtual Product Product { get; set; } = default!;
}