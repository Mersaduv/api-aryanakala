namespace ApiAryanakala.Entities.Product;

public class ProductAttribute
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Title { get; set; }
    public string Value { get; set; }
    public Product Products { get; set; }
}
