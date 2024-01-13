using ApiAryanakala.Models;

namespace ApiAryanakala.Entities.Product;

public class Category : BaseClass<int>
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public List<CategoryImage>? Images { get; set; }
    public int? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }
    public virtual List<Category>? ChildCategories { get; set; }

    public virtual List<Product> Products { get; private set; }
}