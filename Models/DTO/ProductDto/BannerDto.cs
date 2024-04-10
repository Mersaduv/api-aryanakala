namespace ApiAryanakala.Models.DTO.ProductDto;

public class BannerDto : BaseClass<Guid>
{
    public int CategoryId { get; set; }
    public EntityImageDto? Image { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Uri { get; set; }
    public bool IsPublic { get; set; }
    public string Type { get; set; } = string.Empty;
}