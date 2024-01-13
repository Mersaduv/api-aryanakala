using ApiAryanakala.Data;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto;
using ApiAryanakala.Models.DTO.ProductDto.Category;
using ApiAryanakala.Repository;
using ApiAryanakala.Utility;
using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Services.Product;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly GenericRepository<Category> _genericRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ByteFileUtility _byteFileUtility;

    public CategoryService(ApplicationDbContext context, IUnitOfWork unitOfWork,
     ByteFileUtility byteFileUtility)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _byteFileUtility = byteFileUtility;
    }

    public async Task<ServiceResponse<CategoryDTO>> Add(CategoryCreateDTO categoryDto)
    {
        if ((GetBy(categoryDto.Id).GetAwaiter().GetResult()).Data != null)
        {
            return new ServiceResponse<CategoryDTO>
            {
                Data = new CategoryDTO(),
                Success = false,
                Message = "Category Name/Id already Exists"
            };
        }
        var category = new Category
        {
            Id = categoryDto.Id,
            Images = _byteFileUtility.SaveFileInFolder<CategoryImage>(categoryDto.Thumbnail, nameof(Category), false),
            Name = categoryDto.Name,
            Url = categoryDto.Url,
            ParentCategoryId = categoryDto.ParentCategoryId
        };
        await _context.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        // Project the added category to a DTO before returning
        var categoryDTO = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            Url = category.Url,
            ImagesSrc = _byteFileUtility.GetEncryptedFileActionUrl(category.Images.Select(img => img.ThumbnailFileName).ToList(), nameof(Category)),
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
        var category = await GetCategoryAsyncBy(id);
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
        var categories = await GetCategoryAsync();

        var categoryDTOs = categories.Select(c => new CategoryDTO
        {
            Id = c.Id,
            Name = c.Name,
            Url = c.Url,
            ImagesSrc = _byteFileUtility.GetEncryptedFileActionUrl(c.Images.Select(img => img.ThumbnailFileName).ToList(), nameof(Category)),
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
        var count = await _genericRepository.GetTotalCountAsync();
        return new ServiceResponse<List<CategoryDTO>>
        {
            Count = count,
            Data = categoryDTOs
        };
    }


    public async Task<ServiceResponse<CategoryDTO?>> GetBy(int id)
    {
        var category = await GetCategoryAsyncBy(id);

        if (category != null)
        {
            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Url = category.Url,
                ImagesSrc = _byteFileUtility.GetEncryptedFileActionUrl(category.Images.Select(img => img.ThumbnailFileName).ToList(), nameof(Category)),
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

    public async Task<ServiceResponse<CategoryDTO>> Update(CategoryUpdateDTO categoryDto)
    {
        var dbCategory = await GetCategoryAsyncBy(categoryDto.Id);
        if (dbCategory == null)
        {
            return new ServiceResponse<CategoryDTO>
            {
                Data = null,
                Success = false,
                Message = "Category not found."
            };
        }

        dbCategory.Name = categoryDto.Name;
        dbCategory.Url = categoryDto.Url;
        _context.Update(dbCategory);
        await _unitOfWork.SaveChangesAsync();

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


    public async Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetAllCategories(int? page, int? pageSize)
    {
        var categories = await _context.Categories.Include(c => c.ParentCategory).Include(c => c.Images)
            .ToListAsync();

        var result = categories.Select(c => new CategoryDTO
        {
            Id = c.Id,
            Name = c.Name,
            Url = c.Url,
            ParentCategoryId = c.ParentCategoryId,
            ImagesSrc = _byteFileUtility.GetEncryptedFileActionUrl(c.Images.Select(img => img.ThumbnailFileName).ToList(), nameof(Category)),
            ChildCategories = c.ChildCategories?.Select(cc => new CategoryDTO
            {
                Id = cc.Id,
                Name = cc.Name,
                Url = cc.Url,
                ParentCategoryId = cc.ParentCategoryId,
                // Omit ChildCategories here to avoid circular reference
            }).ToList(),
        });
        var count = await _genericRepository.GetTotalCountAsync();

        return new ServiceResponse<IEnumerable<CategoryDTO>>
        {
            Count = count,
            Data = result
        };
    }

    public async Task<List<Category>> GetCategoryAsync() => await _context.Categories
                                                            .Include(c => c.ChildCategories)
                                                            .Include(c => c.Images)
                                                            .AsNoTracking()
                                                            .ToListAsync();
    public async Task<Category> GetCategoryAsyncBy(int id) => await _context.Categories
            .Include(c => c.ChildCategories)
            .Include(c => c.Images)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
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

    public async Task<ServiceResponse<bool>> UpsertCategoryImages(Thumbnails thumbnails, int id)
    {
        var dbCategory = await GetCategoryAsyncBy(id);

        if (dbCategory is null)
        {
            return new ServiceResponse<bool> { Success = false, Message = $"{nameof(Category)} not found." };
        }
        dbCategory.Images.AddRange(_byteFileUtility.SaveFileInFolder<CategoryImage>(thumbnails.Thumbnail, nameof(Category), false));
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }

    public async Task<ServiceResponse<bool>> DeleteCategoryImages(string fileName)
    {
        var categoryImageFromStore = _context.CategoryImages.FirstOrDefault(p => p.ThumbnailFileName == fileName);
        if (categoryImageFromStore != null)
        {
            _context.CategoryImages.Remove(categoryImageFromStore);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Data = false,
                Message = "Image not found."
            };
        }
        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }

}