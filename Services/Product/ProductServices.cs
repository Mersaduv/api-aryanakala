using System.Collections.Generic;
using System.Transactions;
using ApiAryanakala.Data;
using ApiAryanakala.Entities.Exceptions;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.LinQ;
using ApiAryanakala.Mapper;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto;
using ApiAryanakala.Models.DTO.ProductDto.Category;
using ApiAryanakala.Models.RequestQuery;
using ApiAryanakala.Utility;
using Microsoft.EntityFrameworkCore;
namespace ApiAryanakala.Services.Product;

public class ProductServices : IProductServices
{
    private readonly IProductRepository _productRepository;
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ByteFileUtility _byteFileUtility;
    private readonly IGenericRepository<Brand> _brandRepository;
    private readonly IAuthServices _authServices;
    private readonly IHttpContextAccessor _httpContext;

    public ProductServices(IProductRepository productRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,
    ByteFileUtility byteFileUtility,
    IGenericRepository<Brand> brandRepository,
    ApplicationDbContext applicationDbContext,
    IAuthServices authServices, IHttpContextAccessor httpContext)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _byteFileUtility = byteFileUtility;
        _brandRepository = brandRepository;
        _context = applicationDbContext;
        _authServices = authServices;
        _httpContext = httpContext;

    }


    public async Task<ServiceResponse<bool>> Create(ProductCreateDTO productCreateDTO)
    {
        if (_productRepository.GetAsyncBy(productCreateDTO.Title).GetAwaiter().GetResult() != null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Success = false,
                Message = "Product Name/Code already Exists"
            };
        }
        var product = productCreateDTO.ToProducts(_byteFileUtility);
        await _productRepository.CreateAsync(product);

        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }

    public async Task<ServiceResponse<bool>> Delete(Guid id)
    {
        var success = false;
        using (var scope = new TransactionScope())
        {
            var productFromStore = await _productRepository.GetAsyncBy(id);
            if (productFromStore != null)
            {
                try
                {
                    _productRepository.Remove(productFromStore);
                    await _unitOfWork.SaveChangesAsync();
                    scope.Complete();
                    success = true;
                }
                catch (Exception ex)
                {

                    throw new CoreException(ex.Message);
                }
            }
        }
        return new ServiceResponse<bool> { Data = success };
    }

    public async Task<ServiceResponse<bool>> Edit(ProductUpdateDTO productUpdateDTO)
    {
        var success = false;
        var productFromStore = await _productRepository.GetAsyncBy(productUpdateDTO.Id);
        var product = productUpdateDTO.ToProduct();
        if (productUpdateDTO != null)
        {
            using (var scope = new TransactionScope())
            {
                if (productFromStore != null)
                {
                    try
                    {
                        product.Images = productFromStore.Images;
                        product.LastUpdated = DateTime.UtcNow;
                        _productRepository.Update(product);
                        await _unitOfWork.SaveChangesAsync();
                        scope.Complete();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        return new ServiceResponse<bool>
                        {
                            Data = success,
                            Success = success,
                            Message = ex.Message
                        };
                    }
                }
            }
        }
        else
        {
            return new ServiceResponse<bool>
            {
                Data = success,
                Success = success,
                Message = "Product not found."
            };
        }
        return new ServiceResponse<bool>
        {
            Data = success
        };
    }

    public async Task<ServiceResponse<GetAllResponse>> GetAll()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            var count = products.Count();

            var resultList = products
                .Select(product =>
                {
                    var categoryLevelIds = GetCategoryLevelIds(product.Category!);
                    var categories = _context.Categories
                        .Where(c => categoryLevelIds.Contains(c.Id))
                        .ToList();

                    var result = product.ToProductResponse(_byteFileUtility);
                    result.CategoryLevels = categories.Select(c => new CategoryDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Url = c.Url,
                        Level = c.Level,
                    }).ToList();
                    result.CategoryList = categoryLevelIds;

                    return result;
                })
                .ToList();

            var response = new GetAllResponse
            {
                Products = resultList
            };

            return new ServiceResponse<GetAllResponse>
            {
                Data = response,
                Count = count
            };
        }
        catch (Exception)
        {
            return new ServiceResponse<GetAllResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving products."
            };
        }
    }

    private List<int?> GetCategoryLevelIds(Category category)
    {
        var categoryLevelIds = new List<int?>();

        if (category?.Level == 1)
        {
            categoryLevelIds.Add(category.Id);
        }
        else if (category?.Level == 2)
        {
            categoryLevelIds.Add(category.ParentCategoryId);
            categoryLevelIds.Add(category.Id);
        }
        else
        {
            categoryLevelIds.Add(category?.ParentCategory?.ParentCategoryId);
            categoryLevelIds.Add(category?.ParentCategoryId);
            categoryLevelIds.Add(category?.Id);
        }

        return categoryLevelIds;
    }

    public async Task<ServiceResponse<GetAllResponse>> GetProductsBy(string categoryUrl)
    {
        var response = await _productRepository.GetProductsAsyncBy(categoryUrl);
        var result = response.ToProductsResponse(_byteFileUtility);

        return new ServiceResponse<GetAllResponse>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<ProductDTO>> GetSingleProductBy(Guid id)
    {
        try
        {
            var product = await _productRepository.GetAsyncBy(id);

            if (product == null)
            {
                return new ServiceResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Product not found."
                };
            }

            var categoryLevelIds = GetCategoryLevelIds(product.Category!);
            var categories = await _context.Categories
                .Where(c => categoryLevelIds.Contains(c.Id))
                .ToListAsync();

            var result = product.ToProductResponse(_byteFileUtility);
            result.CategoryLevels = categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                Url = c.Url,
                Level = c.Level,
            });
            result.CategoryList = categoryLevelIds;

            return new ServiceResponse<ProductDTO>
            {
                Data = result
            };
        }
        catch (Exception)
        {
            return new ServiceResponse<ProductDTO>
            {
                Success = false,
                Message = "An error occurred while retrieving the product."
            };
        }
    }

    public async Task<ServiceResponse<ProductSearchResult>> SearchProducts(RequestSearchQuery parameters)
    {
        var response = await _productRepository.SearchProductsAsync(parameters.SearchText, parameters.Page);

        return new ServiceResponse<ProductSearchResult>
        {
            Data = response
        };
    }

    public async Task<ServiceResponse<PagingModel<ProductDTO>>> GetProductQuery(RequestQuery requestQuery)
    {
        var query = _context.Products.AsNoTracking();

        // Filter by price range
        if (requestQuery.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= requestQuery.MinPrice.Value);
        }
        if (requestQuery.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= requestQuery.MaxPrice.Value);
        }

        // Sorting
        if (!string.IsNullOrEmpty(requestQuery.SortBy) && !string.IsNullOrEmpty(requestQuery.Sort))
        {
            switch (requestQuery.Sort.ToLower())
            {
                case "asc":
                    query = query.OrderBy(p => EF.Property<object>(p, requestQuery.SortBy));
                    break;
                case "desc":
                    query = query.OrderByDescending(p => EF.Property<object>(p, requestQuery.SortBy));
                    break;
                default:
                    // Set a default sorting if invalid sort value is provided
                    query = query.OrderBy(p => p.Id);
                    break;
            }
        }
        else
        {
            // Set a default sorting if sorting parameters are not provided
            query = query.OrderBy(p => p.Id);
        }

        var totalCount = await query.CountAsync();

        // Paging
        if (requestQuery.Page.HasValue && requestQuery.PageSize > 0)
        {
            query = query.Skip((requestQuery.Page.Value - 1) * requestQuery.PageSize).Take(requestQuery.PageSize);
        }

        var products = await query.ToListAsync();
        var response = products.ToProductsResponse(_byteFileUtility);

        return new ServiceResponse<PagingModel<ProductDTO>>
        {
            Data = new PagingModel<ProductDTO>(response.Products, totalCount, requestQuery.Page ?? 1, requestQuery.PageSize)
        };
    }



    public async Task<ServiceResponse<List<string>>> SearchSuggestions(string searchText)
    {
        var response = await _productRepository.GetProductSearchSuggestionsAsync(searchText);

        return new ServiceResponse<List<string>>
        {
            Data = response
        };
    }

    public async Task<ServiceResponse<bool>> UpsertProductImages(Thumbnails thumbnails, Guid id)
    {
        var userId = _authServices.GetUserId();
        var dbProduct = await _productRepository.GetAsyncBy(id);

        if (dbProduct is null)
        {
            return new ServiceResponse<bool> { Success = false, Message = $"{nameof(Product)} not found." };
        }
        List<EntityImage<Guid, Entities.Product.Product>> imageList = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Entities.Product.Product>>(thumbnails.Thumbnail!, nameof(Product), false);
        dbProduct.Images.AddRange(imageList);
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }

    public ServiceResponse<bool> DeleteProductImages(string fileName)
    {
        var productImageFromStore = _context.ProductImages.FirstOrDefault(p => p.ImageUrl == fileName);
        var userId = _authServices.GetUserId();
        if (productImageFromStore != null)
        {
            _context.ProductImages.Remove(productImageFromStore);
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

    public async Task<ServiceResponse<int>> AddBrand(Brand brand)
    {
        if (await _brandRepository.GetAsyncBy(brand.Id) != null)
        {
            return new ServiceResponse<int>
            {
                Data = 0,
                Success = false,
                Message = "Brand Name/Id already Exists"
            };
        }

        var result = await _brandRepository.Add(brand);

        return new ServiceResponse<int>
        {
            Data = result,
        };
    }

    public async Task<ServiceResponse<bool>> DeleteBrand(int id)
    {
        var success = false;
        var brand = (await GetBrandBy(id)).Data;
        if (brand != null)
        {
            await _brandRepository.Delete(brand.Id);
            await _unitOfWork.SaveChangesAsync();
            success = true;
        }
        return new ServiceResponse<bool>
        {
            Data = success
        };
    }

    public async Task<ServiceResponse<Brand>> GetBrandBy(int id)
    {
        var result = await _brandRepository.GetAsyncBy(id);
        return new ServiceResponse<Brand>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<Brand>>> GetBrands()
    {
        var result = await _brandRepository.GetAllAsync();
        var count = result.Count;
        return new ServiceResponse<IReadOnlyList<Brand>>
        {
            Count = count,
            Data = result
        };
    }

    public async Task<ServiceResponse<int>> UpdateBrand(Brand brand)
    {
        brand.LastUpdated = DateTime.UtcNow;
        var result = await _brandRepository.Update(brand);

        return new ServiceResponse<int>
        {
            Data = result,
        };
    }

}