using ApiAryanakala.Models;

namespace ApiAryanakala.Entities.Product;

public class Slider : BaseClass<Guid>
{
    public int CategoryId { get; set; }
    public EntityImage<Guid, Slider>? Image { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Uri { get; set; }
    public bool IsPublic { get; set; }
}