using ApiAryanakala.Entities.Product;

namespace ApiAryanakala.Models.DTO.ProductDto.Category;

public class CategoryUpdateDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public Colors? Colors { get; set; }
    public int? ParentCategoryId { get; set; }
    public int Level { get; set; }
}