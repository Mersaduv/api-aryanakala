namespace ApiAryanakala.Models.DTO.ProductDto;

public class SliderCreateDto
{
    public int CategoryId { get; set; }
    public required IFormFile Url { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
}