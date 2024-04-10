namespace ApiAryanakala.Models.DTO.ProductDto;

public class SliderDto : BaseClass<Guid>
{
    public int CategoryId { get; set; }
    public EntityImageDto? Image { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
}