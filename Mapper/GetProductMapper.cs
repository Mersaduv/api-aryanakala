using ApiAryanakala.Entities;
using ApiAryanakala.Models.DTO.ProductDtos;
using ApiAryanakala.Utility;

namespace ApiAryanakala.Mapper;

public static class GetProductMapper
{
    public static ProductDTO ToProductResponse(this Product product, ByteFileUtility byteFileUtility)
    {
        return new ProductDTO
        {
            Id = product.Id,
            Title = product.Title,
            Code = product.Code,
            ImagesSrc = byteFileUtility.GetEncryptedFileActionUrl(product.Images, nameof(Product)),

            Info = product.Info.Select(infoDto => new InfoDto
            {
                Title = infoDto.Title,
                Value = infoDto.Value,
            }).ToList(),
            CategoryId = product.CategoryId,
            // Category = product.Category,
            Description = product.Description,
            Discount = product.Discount,
            InStock = product.InStock,
            Price = product.Price,
            Rating = product.Rating,
            Size = product.Size,
            // Colors = product.Colors,
            Slug = product.Slug,
            Sold = product.Sold,
            Created = product.Created,
            LastUpdated = product.LastUpdated
        };
    }
    public static GetAllResponse ToProductsResponse(this IEnumerable<Product> products, ByteFileUtility byteFileUtility)
    {
        return new GetAllResponse
        {
            Products = products.Select(prod => new ProductDTO
            {
                Id = prod.Id,
                Title = prod.Title,
                ImagesSrc = byteFileUtility.GetEncryptedFileActionUrl(prod.Images, nameof(Product)),
                Code = prod.Code,
                Info = prod.Info.Select(infoDto => new InfoDto
                {
                    Title = infoDto.Title,
                    Value = infoDto.Value,
                }).ToList(),
                // Category = prod.Category,
                CategoryId = prod.CategoryId,
                Description = prod.Description,
                Discount = prod.Discount,
                InStock = prod.InStock,
                Price = prod.Price,
                Rating = prod.Rating,
                Size = prod.Size,
                // Colors = prod.Colors,
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