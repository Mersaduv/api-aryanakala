using ApiAryanakala.Entities;

namespace ApiAryanakala.Interfaces.IServices;

public interface ICategoryService
{
    Task<List<Category>> GetAll();
    Task<Category> GetBy(int id);
    Task<Category> Add(Category category);
    Task<Category> Update(Category category);
    Task<bool> Delete(int id);
}