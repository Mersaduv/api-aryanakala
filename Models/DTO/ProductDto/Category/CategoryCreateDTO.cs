using System.Reflection;

namespace ApiAryanakala.Models.DTO.ProductDto.Category;

public class CategoryCreateDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public List<IFormFile> Thumbnail { get; set; }
    public int? ParentCategoryId { get; set; }

    public static async ValueTask<CategoryCreateDTO?> BindAsync(HttpContext context,
                                                             ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;

        var id = Convert.ToInt32(form["Id"]);
        var name = form["Name"];
        var url = form["Url"];
        var parentCategoryId = string.IsNullOrEmpty(form["ParentCategoryId"]) ? null : (int?)int.Parse(form["ParentCategoryId"]);

        return new CategoryCreateDTO
        {
            Id = id,
            Name = name,
            Url = url,
            ParentCategoryId = parentCategoryId,
            Thumbnail = thumbnail,
        };

    }
}