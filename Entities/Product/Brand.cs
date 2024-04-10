using ApiAryanakala.Models;

namespace ApiAryanakala.Entities.Product;

public class Brand : BaseClass<int>
{
    public string Name { get; set; } = string.Empty;
    public virtual List<Product>? Products { get; set; }
}