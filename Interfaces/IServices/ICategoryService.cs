using ApiAryanakala.Entities;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDtos.Category;
using X.PagedList;

namespace ApiAryanakala.Interfaces.IServices;

public interface ICategoryService
{
    Task<ServiceResponse<List<CategoryDTO>>> GetAll();
    Task<ServiceResponse<CategoryDTO?>> GetBy(int id);
    Task<ServiceResponse<CategoryDTO>> Add(Category category);
    Task<ServiceResponse<CategoryDTO>> Update(Category category);
    Task<ServiceResponse<bool>> Delete(int id);
    IEnumerable<int> GetAllChildCategoriesHelper(int parentCategoryId, List<Category> allCategories, List<Category> allChildCategories);
    IEnumerable<int> GetAllChildCategories(int parentCategoryId);
    Task<ServiceResponse<IPagedList<CategoryDTO>>> GetAllCategories(int? page, int pageSize);

}