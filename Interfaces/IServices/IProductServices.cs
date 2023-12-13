using ApiAryanakala.Entities;
using ApiAryanakala.Mapper;
using ApiAryanakala.Mapper;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDtos;

namespace ApiAryanakala.Interfaces.IServices;

public interface IProductServices
{
    Task<ProductCreateDTO> Create(ProductCreateDTO command);
    Task<bool> Edit(ProductUpdateDTO command);
    Task<bool> Delete(Guid id);
    Task<GetAllResponse> GetAll();
    Task<ProductDTO> GetSingleProductBy(Guid id);
    Task<GetAllResponse> GetProductsBy(string categoryUrl);
    Task<ProductSearchResult> SearchProducts(RequestSearchQueryParameters parameters);
    Task<List<string>> SearchSuggestions(string searchText);
}