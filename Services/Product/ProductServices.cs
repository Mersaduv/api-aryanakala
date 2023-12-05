using System.Transactions;
using ApiAryanakala.Entities.Exceptions;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Mapper.Query;
using ApiAryanakala.Mapper.Write;
using ApiAryanakala.Models.DTO.ProductDtos;
using ApiAryanakala.Utility;
namespace ApiAryanakala.Services.Product;

public class ProductServices : IProductServices
{
    private readonly IProductRepository productRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ByteFileUtility byteFileUtility;

    public ProductServices(IProductRepository productRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,
    ByteFileUtility byteFileUtility)
    {
        this.productRepository = productRepository;
        this.unitOfWork = unitOfWork;
        this.httpContextAccessor = httpContextAccessor;
        this.byteFileUtility = byteFileUtility;
    }
    public async Task<ProductCreateDTO> Create(ProductCreateDTO command)
    {
        var product = command.ToProducts(byteFileUtility);
        await productRepository.CreateAsync(product);

        await unitOfWork.SaveChangesAsync();

        var productDTO = product.ToCreateResponse();

        return productDTO;
    }

    public async Task<bool> Delete(Guid id)
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
        }
        return success;
    }

    public async Task<bool> Edit(ProductUpdateDTO command)
    {
        var success = false;
        var product = command.ToProduct(byteFileUtility);
        if (command != null)
        {
            using (var scope = new TransactionScope())
            {
                var productById = productRepository.GetAsyncBy(command.Id);
                if (productById != null)
                {
                    try
                    {
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

        return success;
    }


    public async Task<GetAllResponse> GetAll()
    {
        var products = await productRepository.GetAllAsync();
        var result = products.ToProductsResponse(byteFileUtility);
        return result;
    }

    public async Task<ProductDTO> GetSingleProductBy(Guid id)
    {
        var product = await productRepository.GetAsyncBy(id);
        var result = product.ToProductResponse(byteFileUtility);
        return result;
    }

}