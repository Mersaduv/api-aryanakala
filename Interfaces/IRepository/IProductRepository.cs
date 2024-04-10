using ApiAryanakala.Entities.Product;

namespace ApiAryanakala.Interfaces.IRepository;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<int> GetTotalCountAsync();
    Task<Product?> GetAsyncBy(Guid id);
    Task<Product?> GetAsyncBy(string productName);
    Task CreateAsync(Product product);
    void Update(Product product);
    void Remove(Product product);
    Task<IEnumerable<Product>> GetProductsAsyncBy(string categoryUrl);
    Task<ProductSearchResult> SearchProductsAsync(string searchText, int page);
    Task<List<string>> GetProductSearchSuggestionsAsync(string searchText);

}