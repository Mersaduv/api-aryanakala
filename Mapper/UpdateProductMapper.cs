using ApiAryanakala.Entities.Product;
using ApiAryanakala.Models.DTO.ProductDto;

namespace ApiAryanakala.Mapper;

public static class UpdateProductMapper
{
    public static Product ToProduct(this ProductUpdateDTO product_U_DTO)
    {
        return new Product
        {
            Id = product_U_DTO.Id,
            Title = product_U_DTO.Title,
            Code = product_U_DTO.Code,
            CategoryId = product_U_DTO.CategoryId,
            BrandId = product_U_DTO.BrandId,
            Description = product_U_DTO.Description,
            Discount = product_U_DTO.Discount,
            Info = product_U_DTO.Info?.Select(info => new ProductInfo
            {
                Title = info.Title,
                Value = info.Value,
            }).ToList(),
            Specifications = product_U_DTO.Specification?.Select(info => new ProductSpecification
            {
                Title = info.Title,
                Value = info.Value,
            }).ToList(),
            Colors = product_U_DTO.Colors,
            InStock = product_U_DTO.InStock,
            Price = product_U_DTO.Price,
            Size = product_U_DTO.Size,
            Slug = product_U_DTO.Slug,
            Sold = product_U_DTO.Sold,
            LastUpdated = DateTime.UtcNow,
        };
    }

    public static ProductUpdateDTO ToUpdateResponse(this Product product)
    {
        return new ProductUpdateDTO
        {
            Id = product.Id,
            Title = product.Title,
            Code = product.Code,
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            Description = product.Description,
            Discount = product.Discount,
            Info = product.Info?.Select(infoDto => new ProductAttributeDto
            {
                Title = infoDto.Title,
                Value = infoDto.Value,
            }).ToList(),
            Specification = product.Specifications?.Select(infoDto => new ProductAttributeDto
            {
                Title = infoDto.Title,
                Value = infoDto.Value,
            }).ToList(),
            InStock = product.InStock,
            Price = product.Price,
            Size = product.Size,
            Slug = product.Slug,
            Sold = product.Sold,
        };
    }
}