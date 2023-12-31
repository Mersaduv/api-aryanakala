using ApiAryanakala.Entities;
using ApiAryanakala.Models.DTO.ProductDtos;
using ApiAryanakala.Utility;

namespace ApiAryanakala.Mapper;

public static class CreateProductMapper
{
    public static Product ToProducts(this ProductCreateDTO product_C_DTO, ByteFileUtility byteFileUtility)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Title = product_C_DTO.Title,
            Images = byteFileUtility.SaveFileInFolder(product_C_DTO.Thumbnail, nameof(Product), false),//!Boolean true is encrypted and Boolean false is not encrypted
            Code = product_C_DTO.Code,
            CategoryId = product_C_DTO.CategoryId,
            Description = product_C_DTO.Description,
            Discount = product_C_DTO.Discount,
            Info = product_C_DTO.Info.Select(info => new Info
            {
                Title = info.Title,
                Value = info.Value,
            }).ToList(),
            // Colors = product_C_DTO.Colors,
            InStock = product_C_DTO.InStock,
            Price = product_C_DTO.Price,
            Rating = product_C_DTO.Rating,
            Size = product_C_DTO.Size,
            Slug = product_C_DTO.Slug,
            Sold = product_C_DTO.Sold,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
        };
    }
    public static ProductCreateDTO ToCreateResponse(this Product product)
    {
        return new ProductCreateDTO
        {
            Title = product.Title,
            Code = product.Code,
            CategoryId = product.CategoryId,
            Description = product.Description,
            Discount = product.Discount,
            Info = product.Info.Select(infoDto => new InfoDto
            {
                Title = infoDto.Title,
                Value = infoDto.Value,
            }).ToList(),
            // Colors = product.Colors,
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

}