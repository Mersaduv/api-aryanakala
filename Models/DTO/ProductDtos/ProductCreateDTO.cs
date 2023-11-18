using System.Reflection;

namespace ApiAryanakala.Models.DTO.ProductDtos;

public class ProductCreateDTO
{
    public string Title { get; set; }
    public string Code { get; set; }
    public string Slug { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public double? Discount { get; set; }
    public IFormFile Thumbnail { get; set; }
    public List<string> Category { get; set; }
    public List<string> Size { get; set; }
    public List<InfoDto> Info { get; set; }
    public int InStock { get; set; }
    public int? Sold { get; set; }
    public double Rating { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastUpdated { get; set; }

    public static async ValueTask<ProductCreateDTO?> BindAsync(HttpContext context,
                                               ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();
        var file = form.Files["file"];
        return new ProductCreateDTO
        {
            Thumbnail = file
        };
    }
}

