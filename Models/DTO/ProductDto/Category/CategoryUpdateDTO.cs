namespace ApiAryanakala.Models.DTO.ProductDto.Category;

public class CategoryUpdateDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public int? ParentCategoryId { get; set; }
}