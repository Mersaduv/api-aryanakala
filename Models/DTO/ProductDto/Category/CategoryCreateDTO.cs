using System.Reflection;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Utility;

namespace ApiAryanakala.Models.DTO.ProductDto.Category;

public class CategoryCreateDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public List<IFormFile>? Thumbnail { get; set; }
    public int? ParentCategoryId { get; set; }
    public Colors? Colors { get; set; }
    public int Level { get; set; }

    public static async ValueTask<CategoryCreateDTO?> BindAsync(HttpContext context,
                                                             ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;

        var id = Convert.ToInt32(form["Id"]);
        var name = form["Name"];
        var url = form["Url"];
        var parentCategoryId = string.IsNullOrEmpty(form["ParentCategoryId"]) ? null : (int?)int.Parse(form["ParentCategoryId"]!);
        var level = Convert.ToInt32(form["Level"]);
        var colors = form["Colors"].ToList();
        var categoryColors = ParseHelper.ParseData<Colors>(colors).First();
        return new CategoryCreateDTO
        {
            Id = id,
            Name = name!,
            Url = url!,
            ParentCategoryId = parentCategoryId,
            Level = level,
            Colors = categoryColors,
            Thumbnail = thumbnail,
        };

    }
}