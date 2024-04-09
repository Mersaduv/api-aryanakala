using ApiAryanakala.Models;

namespace ApiAryanakala.Entities.Product;

public class Banner : BaseClass<Guid>
{
    public int CategoryId { get; set; }
    public required EntityImage<Guid, Banner> Image { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public string Type { get; set; } = string.Empty;
}