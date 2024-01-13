namespace ApiAryanakala.Models.DTO.ProductDto.Rating;

public class RatingCreateDTO
{
    public string Comment { get; init; }
    public int Rate { get; init; }
    public string ImageUrl { get; init; }
    public string UserName { get; init; }
    public Guid ProductId { get; init; }
}