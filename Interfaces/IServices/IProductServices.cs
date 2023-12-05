using ApiAryanakala.Entities;
using ApiAryanakala.Mapper.Query;
using ApiAryanakala.Models.DTO.ProductDtos;

namespace ApiAryanakala.Interfaces.IServices;

public interface IProductServices
{
    Task<ProductCreateDTO> Create(ProductCreateDTO command);
    Task<bool> Edit(ProductUpdateDTO command);
    Task<bool> Delete(Guid id);
    Task<GetAllResponse> GetAll();

    Task<ProductDTO> GetSingleProductBy(Guid id);
}