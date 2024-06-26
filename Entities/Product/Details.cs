using ApiAryanakala.Enums;
using ApiAryanakala.Models;

namespace ApiAryanakala.Entities.Product;

public class Details : BaseClass<Guid>
{
    public int CategoryId { get; set; }
    public List<ProductInfo>? Info { get; set; }
    public List<ProductSpecification>? Specifications { get; set; }
    public OptionType OptionType { get; set; }
    public virtual Category? Category { get; set; }
}