using ApiAryanakala.Entities;

namespace ApiAryanakala.Interfaces.IRepository;

public interface IGenericRepository<T> where T : BaseClass
{
    Task<T> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<T> GetByIdWithSpec(ISpecification<T> spec);
    Task<IReadOnlyList<T>> GetAllWithSpec(ISpecification<T> spec);
    Task<int> CountASync(ISpecification<T> spec);
}