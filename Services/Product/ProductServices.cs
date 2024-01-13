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
using ApiAryanakala.Utility;
namespace ApiAryanakala.Services.Product;

public class ProductServices : IProductServices
{
    private readonly IProductRepository productRepository;
    private readonly ApplicationDbContext applicationDbContext;
    private readonly IUnitOfWork unitOfWork;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ByteFileUtility byteFileUtility;
    private readonly IGenericRepository<Brand> brandRepository;
    private readonly IAuthServices authServices;


    public ProductServices(IProductRepository productRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,
    ByteFileUtility byteFileUtility,
    IGenericRepository<Brand> brandRepository,
    ApplicationDbContext applicationDbContext,
    IAuthServices authServices)
    {
        this.productRepository = productRepository;
        this.unitOfWork = unitOfWork;
        this.httpContextAccessor = httpContextAccessor;
        this.byteFileUtility = byteFileUtility;
        this.brandRepository = brandRepository;
        this.applicationDbContext = applicationDbContext;
        this.authServices = authServices;
    }

    public async Task<ServiceResponse<int>> AddBrand(Brand brand)
    {

        if (brandRepository.GetAsyncBy(brand.Id).GetAwaiter().GetResult() != null)
        {
            return new ServiceResponse<int>
            {
                Data = 0,
                Success = false,
                Message = "Brand Name/Id already Exists"
            };
        }

        var result = await brandRepository.Add(brand);

        return new ServiceResponse<int>
        {
            Data = result,
        };
    }


    public async Task<ServiceResponse<ProductCreateDTO>> Create(ProductCreateDTO command)
    {
        if (productRepository.GetAsyncBy(command.Title).GetAwaiter().GetResult() != null)
        {
            return new ServiceResponse<ProductCreateDTO>
            {
                Data = new ProductCreateDTO(),
                Success = false,
                Message = "Product Name/Code already Exists"
            };
        }
        var product = command.ToProducts(byteFileUtility);
        await productRepository.CreateAsync(product);

        await unitOfWork.SaveChangesAsync();

        var productDTO = product.ToCreateResponse();

        return new ServiceResponse<ProductCreateDTO>
        {
            Data = productDTO,
        };
    }

    public async Task<ServiceResponse<bool>> Delete(Guid id)
    {
        var success = false;
        using (var scope = new TransactionScope())
        {
            var productFromStore = await productRepository.GetAsyncBy(id);
            if (productFromStore != null)
            {
                try
                {
                    await productRepository.RemoveAsync(productFromStore);
                    await unitOfWork.SaveChangesAsync();
                    scope.Complete();
                    success = true;
                }
                catch (Exception ex)
                {

                    throw new CoreException(ex.Message);
                }
            }
            else
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Product not found."
                };
            }
        }
        return new ServiceResponse<bool> { Data = success };
    }

    public async Task<ServiceResponse<bool>> DeleteBrand(int id)
    {
        var success = false;
        var brand = (await GetBrandBy(id)).Data;
        if (brand != null)
        {
            await brandRepository.Delete(brand.Id);
            await unitOfWork.SaveChangesAsync();
            success = true;
        }
        return new ServiceResponse<bool>
        {
            Data = success
        };
    }

    public async Task<ServiceResponse<bool>> Edit(ProductUpdateDTO command)
    {
        var success = false;
        var productFromStore = await productRepository.GetAsyncBy(command.Id);
        var product = command.ToProduct();
        if (command != null)
        {
            using (var scope = new TransactionScope())
            {
                if (productFromStore != null)
                {
                    try
                    {
                        product.Images = productFromStore.Images;
                        product.LastUpdated = DateTime.UtcNow;
                        await productRepository.UpdateAsync(product);
                        await unitOfWork.SaveChangesAsync();
                        scope.Complete();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        throw new CoreException(ex.Message);
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
        var products = await productRepository.GetAllAsync();
        var count = await productRepository.GetTotalCountAsync();
        var result = products.ToProductsResponse(byteFileUtility);
        return new ServiceResponse<GetAllResponse>
        {
            Count = count,
            Data = result
        };
    }

    public async Task<ServiceResponse<Brand>> GetBrandBy(int id)
    {
        var result = await brandRepository.GetAsyncBy(id);
        return new ServiceResponse<Brand>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<Brand>>> GetBrands()
    {
        var result = await brandRepository.GetAllAsync();
        var count = await brandRepository.GetTotalCountAsync();
        return new ServiceResponse<IReadOnlyList<Brand>>
        {
            Count = count,
            Data = result
        };
    }


    public async Task<ServiceResponse<GetAllResponse>> GetProductsBy(string categoryUrl)
    {
        var response = await productRepository.GetProductsAsyncBy(categoryUrl);
        var result = response.ToProductsResponse(byteFileUtility);
        return new ServiceResponse<GetAllResponse>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<ProductDTO>> GetSingleProductBy(Guid id)
    {
        var product = await productRepository.GetAsyncBy(id);
        var result = product.ToProductResponse(byteFileUtility);
        return new ServiceResponse<ProductDTO>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<ProductSearchResult>> SearchProducts(RequestSearchQueryParameters parameters)
    {
        var response = await productRepository.SearchProductsAsync(parameters.SearchText, parameters.Page);

        return new ServiceResponse<ProductSearchResult>
        {
            Data = response
        };
    }


    public async Task<ServiceResponse<List<string>>> SearchSuggestions(string searchText)
    {
        var response = await productRepository.GetProductSearchSuggestionsAsync(searchText);

        return new ServiceResponse<List<string>>
        {
            Data = response
        };
    }

    public async Task<ServiceResponse<int>> UpdateBrand(Brand brand)
    {
        var result = await brandRepository.Update(brand);

        return new ServiceResponse<int>
        {
            Data = result,
        };
    }

    public async Task<ServiceResponse<bool>> UpsertProductImages(Thumbnails thumbnails, Guid id)
    {
        var userId = authServices.GetUserId();
        var dbProduct = await productRepository.GetAsyncBy(id);

        if (dbProduct is null)
        {
            return new ServiceResponse<bool> { Success = false, Message = $"{nameof(Product)} not found." };
        }
        dbProduct.Images.AddRange(byteFileUtility.SaveFileInFolder<ProductImage>(thumbnails.Thumbnail, nameof(Product), false));
        await unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }

    public async Task<ServiceResponse<bool>> DeleteProductImages(string fileName)
    {
        var productImageFromStore = applicationDbContext.ProductImages.FirstOrDefault(p => p.ThumbnailFileName == fileName);
        var userId = authServices.GetUserId();
        if (productImageFromStore != null)
        {
            applicationDbContext.ProductImages.Remove(productImageFromStore);
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