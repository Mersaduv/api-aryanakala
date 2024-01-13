using ApiAryanakala.Entities.Product;
using ApiAryanakala.Models.DTO.ProductDto;
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
            ImagesSrc =byteFileUtility.GetEncryptedFileActionUrl(product.Images.Select(img => img.ThumbnailFileName).ToList(), nameof(Product)),

            ProductAttribute = product.ProductAttribute.Select(infoDto => new ProductAttributeDto
            {
                Title = infoDto.Title,
                Value = infoDto.Value,
            }).ToList(),
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            Description = product.Description,
            Discount = product.Discount,
            InStock = product.InStock,
            Price = product.Price,
            Size = product.Size,
            Colors = product.Colors,
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
                ImagesSrc = byteFileUtility.GetEncryptedFileActionUrl(prod.Images.Select(img => img.ThumbnailFileName).ToList(), nameof(Product)),
                Code = prod.Code,
                ProductAttribute = prod.ProductAttribute.Select(infoDto => new ProductAttributeDto
                {
                    Title = infoDto.Title,
                    Value = infoDto.Value,
                }).ToList(),
                CategoryId = prod.CategoryId,
                BrandId = prod.BrandId,
                Description = prod.Description,
                Discount = prod.Discount,
                InStock = prod.InStock,
                Price = prod.Price,
                Size = prod.Size,
                Colors = prod.Colors,
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