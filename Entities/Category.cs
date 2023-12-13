using ApiAryanakala.Models;

namespace ApiAryanakala.Entities;

public class Category : BaseClass<int>
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public List<Product> Products { get; private set; }
}