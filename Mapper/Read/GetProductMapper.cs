using ApiAryanakala.Entities;
using ApiAryanakala.Models.DTO.ProductDtos;

namespace ApiAryanakala.Mapper.Query;

public static class GetProductMapper
{
    public static ProductDTO ToProductResponse(this Product product)
    {
        return new ProductDTO
        {
            Id = product.Id,
            Title = product.Title,
            Code = product.Code,
            Info = product.Info.Select(infoDto => new InfoDto
            {
                Title = infoDto.Title,
                Value = infoDto.Value,
            }).ToList(),
            Category = product.Category,
            Description = product.Description,
            Discount = product.Discount,
            InStock = product.InStock,
            Price = product.Price,
            Rating = product.Rating,
            Size = product.Size,
            Slug = product.Slug,
            Sold = product.Sold,
            Created = product.Created,
            LastUpdated = product.LastUpdated
        };
    }
    public static GetAllResponse ToProductsResponse(this IEnumerable<Product> products)
    {
        return new GetAllResponse
        {
            Products = products.Select(prod => new ProductDTO
            {
                Id = prod.Id,
                Title = prod.Title,
                Code = prod.Code,
                Info = prod.Info.Select(infoDto => new InfoDto
                {
                    Title = infoDto.Title,
                    Value = infoDto.Value,
                }).ToList(),
                Category = prod.Category,
                Description = prod.Description,
                Discount = prod.Discount,
                InStock = prod.InStock,
                Price = prod.Price,
                Rating = prod.Rating,
                Size = prod.Size,
                Slug = prod.Slug,
                Sold = prod.Sold,
                Created = prod.Created,
                LastUpdated = prod.LastUpdated
            }).ToList()
        };
    }
}

public class GetAllResponse
{
    public IEnumerable<ProductDTO> Products { get; set; } = Enumerable.Empty<ProductDTO>();
}