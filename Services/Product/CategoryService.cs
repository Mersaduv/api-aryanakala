using System.Text.Json;
using System.Text.Json.Serialization;
using ApiAryanakala.Data;
using ApiAryanakala.Entities;
using ApiAryanakala.Entities.Exceptions;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDtos.Category;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace ApiAryanakala.Services.Product;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<CategoryDTO>> Add(Category category)
    {
        if ((GetBy(category.Id).GetAwaiter().GetResult()).Data != null)
        {
            return new ServiceResponse<CategoryDTO>
            {
                Data = new CategoryDTO(),
                Success = false,
                Message = "Category Name/Id already Exists"
            };
        }

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        // Project the added category to a DTO before returning
        var categoryDTO = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            Url = category.Url,
            ParentCategoryId = category.ParentCategoryId,
            // Omit ChildCategories here to avoid circular reference
        };

        return new ServiceResponse<CategoryDTO>
        {
            Data = categoryDTO
        };
    }

    public async Task<ServiceResponse<bool>> Delete(int id)
    {
        var success = false;
        Category category = await _context.Categories
            .Include(c => c.ChildCategories)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            success = true;
        }
        return new ServiceResponse<bool>
        {
            Data = success
        };
    }

    public async Task<ServiceResponse<List<CategoryDTO>>> GetAll()
    {
        var categories = await _context.Categories
            .Include(c => c.ChildCategories)
            .ToListAsync();

        // Project entities to DTOs
        var categoryDTOs = categories.Select(c => new CategoryDTO
        {
            Id = c.Id,
            Name = c.Name,
            Url = c.Url,
            ParentCategoryId = c.ParentCategoryId,
            ChildCategories = c.ChildCategories?.Select(cc => new CategoryDTO
            {
                Id = cc.Id,
                Name = cc.Name,
                Url = cc.Url,
                ParentCategoryId = cc.ParentCategoryId,
                // Omit ChildCategories here to avoid circular reference
            }).ToList()
        }).ToList();

        return new ServiceResponse<List<CategoryDTO>>
        {
            Data = categoryDTOs
        };
    }


    public async Task<ServiceResponse<CategoryDTO?>> GetBy(int id)
    {
        var category = await _context.Categories
            .Include(c => c.ChildCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category != null)
        {
            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Url = category.Url,
                ParentCategoryId = category.ParentCategoryId,
                ChildCategories = category.ChildCategories?.Select(cc => new CategoryDTO
                {
                    Id = cc.Id,
                    Name = cc.Name,
                    Url = cc.Url,
                    ParentCategoryId = cc.ParentCategoryId,
                    // Omit ChildCategories here to avoid circular reference
                }).ToList()
            };

            return new ServiceResponse<CategoryDTO?>
            {
                Data = categoryDTO
            };
        }
        else
        {
            return new ServiceResponse<CategoryDTO?>
            {
                Data = null,
                Success = false,
                Message = "Category not found."
            };
        }
    }

    public async Task<ServiceResponse<CategoryDTO>> Update(Category category)
    {
        var dbCategory = (await GetBy(category.Id)).Data;
        if (dbCategory == null)
        {
            return new ServiceResponse<CategoryDTO>
            {
                Data = null,
                Success = false,
                Message = "Category not found."
            };
        }

        dbCategory.Name = category.Name;
        dbCategory.Url = category.Url;

        await _context.SaveChangesAsync();

        // Project the updated category to a DTO before returning
        var updatedCategoryDTO = new CategoryDTO
        {
            Id = dbCategory.Id,
            Name = dbCategory.Name,
            Url = dbCategory.Url,
            ParentCategoryId = dbCategory.ParentCategoryId,
            // Omit ChildCategories here to avoid circular reference
        };

        return new ServiceResponse<CategoryDTO>
        {
            Data = updatedCategoryDTO
        };
    }


    public async Task<ServiceResponse<IPagedList<CategoryDTO>>> GetAllCategories(int? page, int pageSize)
    {
        var categories = await _context.Categories.Include(c => c.ParentCategory)
            .ToPagedListAsync<Category>(page ?? 1, pageSize);

        var categoryDTOs = categories.Select(c => new CategoryDTO
        {
            Id = c.Id,
            Name = c.Name,
            Url = c.Url,
            ParentCategoryId = c.ParentCategoryId,
            ChildCategories = c.ChildCategories?.Select(cc => new CategoryDTO
            {
                Id = cc.Id,
                Name = cc.Name,
                Url = cc.Url,
                ParentCategoryId = cc.ParentCategoryId,
                // Omit ChildCategories here to avoid circular reference
            }).ToList(),
        });

        return new ServiceResponse<IPagedList<CategoryDTO>>
        {
            Data = new StaticPagedList<CategoryDTO>(categoryDTOs, categories.GetMetaData())
        };
    }


    public IEnumerable<int> GetAllChildCategories(int parentCategoryId) =>
        GetAllChildCategoriesHelper(parentCategoryId, null, null);

    public IEnumerable<int> GetAllChildCategoriesHelper(int parentCategoryId, List<Category> allCategories, List<Category> allChildCategories)
    {
        if (allCategories == null)
        {
            allCategories = _context.Categories.ToList();
            allChildCategories = new List<Category>();
        }

        var childCategories = allCategories.Where(c => c.ParentCategoryId == parentCategoryId);

        if (childCategories.Count() > 0)
        {
            allChildCategories.AddRange(childCategories.ToList());
            foreach (var childCategory in childCategories)
                GetAllChildCategoriesHelper(childCategory.Id, allCategories, allChildCategories);
        }
        return allChildCategories.Select(c => c.Id);
    }

}