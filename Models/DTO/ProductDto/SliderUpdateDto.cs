namespace ApiAryanakala.Models.DTO.ProductDto;

public class SliderUpdateDto
{
    public Guid Id { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
}