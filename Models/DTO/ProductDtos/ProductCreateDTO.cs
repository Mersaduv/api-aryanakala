using System.Reflection;
using System.Text.Json;
using Newtonsoft.Json;

namespace ApiAryanakala.Models.DTO.ProductDtos;

public class ProductCreateDTO
{
    public string Title { get; set; }
    public string Code { get; set; }
    public string Slug { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public double? Discount { get; set; }
    public List<IFormFile> Thumbnail { get; set; }
    // public List<string> Category { get; set; }
    public int CategoryId { get; set; }
    public List<string> Size { get; set; }
    // public List<string> Colors { get; set; }
    public List<InfoDto> Info { get; set; } = [];
    public int InStock { get; set; }
    public int? Sold { get; set; }
    public double Rating { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastUpdated { get; set; }
    public static async ValueTask<ProductCreateDTO?> BindAsync(HttpContext context,
                                                                ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;
        var title = form["Title"];
        var code = form["Code"];
        var slug = form["Slug"];
        var price = double.Parse(form["Price"]);
        var description = form["Description"];
        var discount = string.IsNullOrEmpty(form["Discount"]) ? null : (double?)double.Parse(form["Discount"]);
        // var categories = form["Category"].ToList();
        var categoryId = int.Parse(form["CategoryId"]);

        var sizes = form["Size"].ToList();
        // var colors = form["Colors"].ToList();
        var infoData = form["Info"];
        var infoList = ParseInfoData(infoData);
        var inStock = int.Parse(form["InStock"]);

        var sold = string.IsNullOrEmpty(form["Sold"]) ? null : (int?)int.Parse(form["Sold"]);
        var rating = double.Parse(form["Rating"]);
        var created = string.IsNullOrEmpty(form["Created"]) ? null : (DateTime?)DateTime.Parse(form["Created"]);
        var lastUpdated = string.IsNullOrEmpty(form["LastUpdated"]) ? null : (DateTime?)DateTime.Parse(form["LastUpdated"]);

        return new ProductCreateDTO
        {
            Thumbnail = thumbnail,
            Title = title,
            Code = code,
            Slug = slug,
            Price = price,
            Description = description,
            Discount = discount,
            CategoryId = categoryId,
            Size = sizes,
            // Colors = colors,
            Info = infoList,
            InStock = inStock,
            Sold = sold,
            Rating = rating,
            Created = created,
            LastUpdated = lastUpdated
        };
    }


    private static List<InfoDto> ParseInfoData(string infoData)
    {
        try
        {
            string jsonString = $"[{infoData}]";

            InfoDto[] myObjectsArray = JsonConvert.DeserializeObject<InfoDto[]>(jsonString);
            List<InfoDto> myObjectsList = new(myObjectsArray);
            return myObjectsList;
        }
        catch (System.Text.Json.JsonException)
        {
            return [];
        }
    }

}

