using ApiAryanakala.Entities;
using ApiAryanakala.Models.DTO.ProductDto;

namespace ApiAryanakala.Mapper.Write;

public static class UpdateProductMapper
{
    public static Product ToProduct(this ProductUpdateDTO product_U_DTO)
    {
        return new Product
        {
            Id = product_U_DTO.Id,
            Title = product_U_DTO.Title,
            Code = product_U_DTO.Code,
            Category = product_U_DTO.Category,
            Description = product_U_DTO.Description,
            Discount = product_U_DTO.Discount,
            Images = product_U_DTO.Images,
            Info = product_U_DTO.Info.Select(info => new Info
            {
                Title = info.Title,
                Value = info.Value,
            }).ToList(),
            InStock = product_U_DTO.InStock,
            Price = product_U_DTO.Price,
            Rating = product_U_DTO.Rating,
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
            LastUpdated = product.LastUpdated
        };
    }
}