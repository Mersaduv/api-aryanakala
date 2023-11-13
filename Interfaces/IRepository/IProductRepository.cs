using ApiAryanakala.Entities;
using ApiAryanakala.Models.DTO.ProductDto;

namespace ApiAryanakala.Interfaces.IRepository;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> GetAsync(Guid id);
    Task<Product> GetAsync(string productName);
    Task CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task RemoveAsync(Product product);
}