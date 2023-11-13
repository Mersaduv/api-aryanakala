using ApiAryanakala.Entities;
using ApiAryanakala.Models.DTO.ProductDto;

namespace ApiAryanakala.Mapper.Write;

public static class CreateProductMapper
{
    public static Product ToProducts(this ProductCreateDTO product_C_DTO)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Title = product_C_DTO.Title,
            Code = product_C_DTO.Code,
            Category = product_C_DTO.Category,
            Description = product_C_DTO.Description,
            Discount = product_C_DTO.Discount,
            Images = product_C_DTO.Images,
            Info = product_C_DTO.Info.Select(info => new Info
            {
                Title = info.Title,
                Value = info.Value,
            }).ToList(),
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
            Category = product.Category,
            Description = product.Description,
            Discount = product.Discount,
            Images = product.Images,
            Info = product.Info.Select(infoDto => new InfoDto
            {
                Title = infoDto.Title,
                Value = infoDto.Value,
            }).ToList(),
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