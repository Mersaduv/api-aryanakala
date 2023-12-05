using ApiAryanakala.Entities;

namespace ApiAryanakala.Interfaces.IRepository;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> GetAsyncBy(Guid id);
    Task<Product> GetAsyncBy(string productName);
    Task CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task RemoveAsync(Product product);
}