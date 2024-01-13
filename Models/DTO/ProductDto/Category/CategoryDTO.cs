using System.Reflection;

namespace ApiAryanakala.Models.DTO.ProductDto.Category;

public class CategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public List<string>? ImagesSrc { get; set; }
    public int? ParentCategoryId { get; set; }
    public CategoryDTO? ParentCategory { get; set; }
    public List<CategoryDTO> ChildCategories { get; set; }
}
