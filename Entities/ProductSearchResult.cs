using ApiAryanakala.Mapper;
using ApiAryanakala.Models.DTO.ProductDtos;

namespace ApiAryanakala.Entities;

public class ProductSearchResult
{
    public GetAllResponse Products { get; set; } = new GetAllResponse();
    public int Pages { get; set; }
    public int CurrentPage { get; set; }
}