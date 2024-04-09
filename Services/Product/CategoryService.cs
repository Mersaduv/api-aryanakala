using ApiAryanakala.Data;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO;
using ApiAryanakala.Models.DTO.ProductDto;
using ApiAryanakala.Models.DTO.ProductDto.Category;
using ApiAryanakala.Utility;
using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Services.Product;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IGenericRepository<Category> _genericRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ByteFileUtility _byteFileUtility;
    private readonly IHttpContextAccessor _httpContext;

    public CategoryService(ApplicationDbContext context, IUnitOfWork unitOfWork,
     ByteFileUtility byteFileUtility, IGenericRepository<Category> genericRepository, IHttpContextAccessor httpContext)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _byteFileUtility = byteFileUtility;
        _genericRepository = genericRepository;
        _httpContext = httpContext;
    }

    public async Task<ServiceResponse<CategoryDTO>> Add(CategoryCreateDTO categoryDto)
    {
        if (GetBy(categoryDto.Id, null).GetAwaiter().GetResult().Data != null)
        {
            return new ServiceResponse<CategoryDTO>
            {
                Data = null,
                Success = false,
                Message = "Category Name/Id already Exists"
            };
        }

        var category = new Category
        {
            Id = categoryDto.Id,
            Images = _byteFileUtility.SaveFileInFolder<EntityImage<int, Category>>(categoryDto.Thumbnail!, nameof(Category), false),
            Name = categoryDto.Name,
            Url = categoryDto.Url,
            ParentCategoryId = categoryDto.ParentCategoryId,
            Colors = new Colors { Start = categoryDto.Colors!.Start, End = categoryDto.Colors.End },
            Level = categoryDto.Level,

        };
        await _context.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        // Project the added category to a DTO before returning
        var categoryDTO = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            Url = category.Url,
            ImagesSrc = _byteFileUtility.GetEncryptedFileActionUrl(category.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl!,
                Placeholder = img.Placeholder!
            }).ToList(), nameof(Category)),
            ParentCategoryId = category.ParentCategoryId,
            Colors = new Colors { Start = category.Colors.Start, End = category.Colors.End },
            Level = category.Level
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
        var category = await GetCategoryAsyncBy(id, null);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _unitOfWork.SaveChangesAsync();
            success = true;
        }
        return new ServiceResponse<bool>
        {
            Data = success
        };
    }
    public async Task<ServiceResponse<CategoryDTO?>> GetBy(int? id, string? slug)
    {
        var category = await GetCategoryAsyncBy(id, slug);

        if (category != null)
        {
            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Url = category.Url,
                ImagesSrc = category.Images is not null ? _byteFileUtility.GetEncryptedFileActionUrl(category.Images.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl!,
                    Placeholder = img.Placeholder!
                }).ToList(), nameof(Category)) : null,
                Colors = new Colors { Start = category.Colors!.Start, End = category.Colors.End },
                ParentCategoryId = category.ParentCategoryId,
                ChildCategories = category.ChildCategories?.Select(cc => new CategoryDTO
                {
                    Id = cc.Id,
                    Name = cc.Name,
                    Url = cc.Url,
                    ParentCategoryId = cc.ParentCategoryId,
                    // Omit ChildCategories here to avoid circular reference
                }).ToList(),
                Level = category.Level,

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
        var dbCategory = await GetCategoryAsyncBy(categoryDto.Id, null);
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
        dbCategory.Colors = categoryDto.Colors;
        dbCategory.ParentCategoryId = categoryDto.ParentCategoryId;
        dbCategory.Level = categoryDto.Level;
        dbCategory.LastUpdated = DateTime.UtcNow;
        _context.Update(dbCategory);
        await _unitOfWork.SaveChangesAsync();

        // Project the updated category to a DTO before returning
        var updatedCategoryDTO = new CategoryDTO
        {
            Id = dbCategory.Id,
            Name = dbCategory.Name,
            Url = dbCategory.Url,
            ParentCategoryId = dbCategory.ParentCategoryId,
            ImagesSrc =dbCategory.Images is not null ? _byteFileUtility.GetEncryptedFileActionUrl(dbCategory.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl!,
                Placeholder = img.Placeholder!
            }).ToList(), nameof(Category)) : null,
            Colors = new Colors { Start = dbCategory.Colors!.Start, End = dbCategory.Colors.End },
            Level = categoryDto.Level,
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
        // tree category
        var result = categories.Select(c1 => new CategoryDTO
        {
            Id = c1.Id,
            Name = c1.Name,
            Url = c1.Url,
            ParentCategoryId = c1.ParentCategoryId,
            ImagesSrc =c1.Images is not null ? _byteFileUtility.GetEncryptedFileActionUrl(c1.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl!,
                Placeholder = img.Placeholder!
            }).ToList(), nameof(Category)) : null,
            Colors = new Colors { Start = c1.Colors!.Start, End = c1.Colors.End },
            Level = c1.Level,
            ChildCategories = c1.ChildCategories?.Select(c2 => new CategoryDTO
            {
                Id = c2.Id,
                Name = c2.Name,
                Url = c2.Url,
                ParentCategoryId = c2.ParentCategoryId,
                ChildCategories = c2.ChildCategories?.Select(c3 => new CategoryDTO
                {
                    Id = c3.Id,
                    Name = c3.Name,
                    Url = c3.Url,
                    ParentCategoryId = c3.ParentCategoryId,
                }).ToList()
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
    public async Task<Category?> GetCategoryAsyncBy(int? id, string? slug)
    {
        return await _context.Categories
            .Include(c => c.ChildCategories)
            .Include(c => c.Images)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => !string.IsNullOrEmpty(slug) ? c.Url == slug : (c.Id == id)) ?? null;;
    }


    public IEnumerable<int> GetAllChildCategories(int parentCategoryId) =>
        GetAllChildCategoriesHelper(parentCategoryId, null, null);

    public IEnumerable<int> GetAllChildCategoriesHelper(int parentCategoryId, List<Category>? allCategories, List<Category>? allChildCategories)
    {
        if (allCategories == null)
        {
            allCategories = _context.Categories.ToList();
            allChildCategories = new List<Category>();
        }

        var childCategories = allCategories.Where(c => c.ParentCategoryId == parentCategoryId);

        if (childCategories.Count() > 0)
        {
            allChildCategories!.AddRange(childCategories.ToList());
            foreach (var childCategory in childCategories)
                GetAllChildCategoriesHelper(childCategory.Id, allCategories, allChildCategories);
        }
        return allChildCategories!.Select(c => c.Id);
    }

    public async Task<ServiceResponse<bool>> UpsertCategoryImages(Thumbnails thumbnails, int id)
    {
        var dbCategory = await GetCategoryAsyncBy(id, null);

        if (dbCategory is null)
        {
            return new ServiceResponse<bool> { Success = false, Message = $"{nameof(Category)} not found." };
        }
        dbCategory.Images!.AddRange(_byteFileUtility.SaveFileInFolder<EntityImage<int, Category>>(thumbnails.Thumbnail!, nameof(Category), false));
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }

    public async Task<ServiceResponse<bool>> DeleteCategoryImages(string fileName)
    {
        var categoryImageFromStore = _context.CategoryImages.FirstOrDefault(p => p.ImageUrl == fileName);
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