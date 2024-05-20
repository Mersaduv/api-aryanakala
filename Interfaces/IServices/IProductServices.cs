using ApiAryanakala.Entities.Product;
using ApiAryanakala.Mapper;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto;
using ApiAryanakala.Models.RequestQuery;

namespace ApiAryanakala.Interfaces.IServices;

public interface IProductServices
{
    Task<ServiceResponse<bool>> Create(ProductCreateDTO productCreateDTO);
    Task<ServiceResponse<bool>> Edit(ProductUpdateDTO productUpdateDTO);
    Task<ServiceResponse<bool>> Delete(Guid id);
    Task<ServiceResponse<GetAllResponse>> GetAll();
    Task<ServiceResponse<bool>> UpsertProductImages(Thumbnails thumbnails, Guid id);
    ServiceResponse<bool> DeleteProductImages(string fileName);
    Task<ServiceResponse<ProductDTO>> GetSingleProductBy(Guid id);
    Task<ServiceResponse<GetAllResponse>> GetProductsBy(string categoryUrl);
    Task<ServiceResponse<ProductSearchResult>> SearchProducts(RequestSearchQuery parameters);
    Task<ServiceResponse<PagingModel<ProductDTO>>> GetProductQuery(RequestQuery requestQuery);
    Task<ServiceResponse<List<string>>> SearchSuggestions(string searchText);
    Task<ServiceResponse<int>> AddBrand(Brand brand);
    Task<ServiceResponse<int>> UpdateBrand(Brand brand);
    Task<ServiceResponse<bool>> DeleteBrand(int id);
    Task<ServiceResponse<Brand>> GetBrandBy(int id);
    Task<ServiceResponse<IReadOnlyList<Brand>>> GetBrands();

}