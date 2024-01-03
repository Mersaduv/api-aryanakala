using ApiAryanakala.Entities;
using ApiAryanakala.Mapper;

namespace ApiAryanakala.Interfaces.IRepository;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<int> GetTotalCountAsync();
    Task<Product> GetAsyncBy(Guid id);
    Task<Product> GetAsyncBy(string productName);
    Task CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task RemoveAsync(Product product);
    Task<bool> UpsertProductImagesAsync(List<Guid> productIds);
    Task<IEnumerable<Product>> GetProductsAsyncBy(string categoryUrl);
    Task<ProductSearchResult> SearchProductsAsync(string searchText, int page);
    Task<List<string>> GetProductSearchSuggestionsAsync(string searchText);

}