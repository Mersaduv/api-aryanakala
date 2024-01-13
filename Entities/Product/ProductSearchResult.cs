using ApiAryanakala.Mapper;

namespace ApiAryanakala.Entities.Product;

public class ProductSearchResult
{
    public GetAllResponse Products { get; set; } = new GetAllResponse();
    public int Pages { get; set; }
    public int CurrentPage { get; set; }
}