namespace ApiAryanakala.Models.DTO;

public class EntityImageDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Placeholder { get; set; } = string.Empty;
}