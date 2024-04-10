using System.Collections.Generic;
using System.Transactions;
using ApiAryanakala.Data;
using ApiAryanakala.Entities.Exceptions;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Mapper;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto;
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


    public async Task<ServiceResponse<ProductDTO>> Create(ProductCreateDTO productCreateDTO)
    {
        if (_productRepository.GetAsyncBy(productCreateDTO.Title).GetAwaiter().GetResult() != null)
        {
            return new ServiceResponse<ProductDTO>
            {
                Data = null,
                Success = false,
                Message = "Product Name/Code already Exists"
            };
        }

        var product = productCreateDTO.ToProducts(_byteFileUtility);
        await _productRepository.CreateAsync(product);

        await _unitOfWork.SaveChangesAsync();

        var productDTO = product.ToCreateResponse();

        var categoryLevelOneId = product.CategoryLevels!.LevelOne;
        var categoryLevelTwoId = product.CategoryLevels.LevelTwo;
        var categoryLevelThreeId = product.CategoryLevels.LevelThree;
        var categoryLevelIds = new List<int> { categoryLevelOneId, categoryLevelTwoId, categoryLevelThreeId };

        var categories = await _context.Categories
            .Where(c => categoryLevelIds.Contains(c.Id))
            .ToListAsync();

        productDTO.CategoryLevels = categories;
        productDTO.CategoryList = categoryLevelIds;

        return new ServiceResponse<ProductDTO>
        {
            Data = productDTO,
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
        var products = await _productRepository.GetAllAsync();
        var count = products.Count();
        var result = products.ToProductsResponse(_byteFileUtility);
        return new ServiceResponse<GetAllResponse>
        {
            Count = count,
            Data = result
        };
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
        var product = await _productRepository.GetAsyncBy(id);
        var result = product.ToProductResponse(_byteFileUtility);
        return new ServiceResponse<ProductDTO>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<ProductSearchResult>> SearchProducts(RequestSearchQuery parameters)
    {
        var response = await _productRepository.SearchProductsAsync(parameters.SearchText, parameters.Page);

        return new ServiceResponse<ProductSearchResult>
        {
            Data = response
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