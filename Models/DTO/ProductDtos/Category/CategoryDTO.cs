namespace ApiAryanakala.Models.DTO.ProductDtos.Category;

public class CategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public int? ParentCategoryId { get; set; }
    public  CategoryDTO? ParentCategory { get; set; }
    public List<CategoryDTO> ChildCategories { get; set; }
}
