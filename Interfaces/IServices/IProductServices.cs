using ApiAryanakala.Entities.Product;
using ApiAryanakala.Mapper;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto;

namespace ApiAryanakala.Interfaces.IServices;

public interface IProductServices
{
    Task<ServiceResponse<ProductCreateDTO>> Create(ProductCreateDTO command);
    Task<ServiceResponse<bool>> Edit(ProductUpdateDTO command);
    Task<ServiceResponse<bool>> Delete(Guid id);
    Task<ServiceResponse<GetAllResponse>> GetAll();
    Task<ServiceResponse<bool>> UpsertProductImages(Thumbnails thumbnails, Guid id);
    Task<ServiceResponse<bool>> DeleteProductImages(string fileName);
    Task<ServiceResponse<ProductDTO>> GetSingleProductBy(Guid id);
    Task<ServiceResponse<GetAllResponse>> GetProductsBy(string categoryUrl);
    Task<ServiceResponse<ProductSearchResult>> SearchProducts(RequestSearchQueryParameters parameters);
    Task<ServiceResponse<List<string>>> SearchSuggestions(string searchText);
    Task<ServiceResponse<int>> AddBrand(Brand brand);
    Task<ServiceResponse<int>> UpdateBrand(Brand brand);
    Task<ServiceResponse<bool>> DeleteBrand(int id);
    Task<ServiceResponse<Brand>> GetBrandBy(int id);
    Task<ServiceResponse<IReadOnlyList<Brand>>> GetBrands();

}