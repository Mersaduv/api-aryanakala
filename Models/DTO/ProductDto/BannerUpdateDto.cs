namespace ApiAryanakala.Models.DTO.ProductDto;

public class BannerUpdateDto
{
    public Guid Id { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Uri { get; set; }
    public bool IsPublic { get; set; }
    public string Type { get; set; } = string.Empty;
}