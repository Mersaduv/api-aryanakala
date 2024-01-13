using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Models;
using ApiAryanakala.Filter;
using ApiAryanakala.Data;
using ApiAryanakala.Utility;
using ApiAryanakala.Models.DTO.ProductDto;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Const;
using Microsoft.AspNetCore.Http.HttpResults;
using ApiAryanakala.Mapper;
using ApiAryanakala.Entities.Product;

namespace ApiAryanakala.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder ConfigureProductEndpoints(this IEndpointRouteBuilder apiGroup)
    {
        var productsGroup = apiGroup.MapGroup(Constants.Products);
        var productGroup = apiGroup.MapGroup(Constants.Product);
        productsGroup.MapGet(string.Empty, GetAllProduct);

        productsGroup.MapGet(Constants.Main, GetProductQuery);

        productGroup.MapGet("/{id:guid}", GetProduct);

        productsGroup.MapGet($"/{Constants.Category}/{{categoryUrl}}", GetProductsBy);

        apiGroup.MapGet($"/{Constants.ImageApi}/{{entity}}/{{fileName}}", MediaEndpoint);

        productsGroup.MapGet($"/{Constants.Search}", GetSearchProducts);

        productsGroup.MapGet($"/{Constants.SearchSuggestions}", GetSearchSuggestions);


        var adminProductGroup = productGroup.MapGroup(string.Empty).RequireAuthorization();
        adminProductGroup.MapPost(string.Empty, CreateProduct)
        .AddEndpointFilter<ModelValidationFilter<ProductCreateDTO>>()
        .Accepts<ProductCreateDTO>("multipart/form-data")
        .ProducesValidationProblem();

        adminProductGroup.MapPut(string.Empty, UpdateProduct)
        .AddEndpointFilter<GuidValidationFilter>()
        .AddEndpointFilter<ModelValidationFilter<ProductUpdateDTO>>()
        .ProducesValidationProblem();

        adminProductGroup.MapPost($"/{Constants.ProductImages}/{{id:Guid}}", UpsertProductImages)
        .Accepts<Thumbnails>("multipart/form-data");

        adminProductGroup.MapDelete($"/{Constants.ProductImages}/{{fileName}}", DeleteProductImage)
        .AddEndpointFilter<GuidValidationFilter>()
        .ProducesValidationProblem();

        adminProductGroup.MapDelete("/{id:guid}", DeleteProduct)
        .AddEndpointFilter<GuidValidationFilter>()
        .ProducesValidationProblem();
        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<bool>>> DeleteProductImage(string fileName, IProductServices productServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Delete Product Images");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await productServices.DeleteProductImages(fileName);
        return TypedResults.Ok(result);
    }


    private static async Task<Ok<ServiceResponse<bool>>> UpsertProductImages(Guid id, Thumbnails thumbnails, IProductServices productServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Upsert Product Images");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await productServices.UpsertProductImages(thumbnails, id);
        return TypedResults.Ok(result);
    }


    private static async Task<Ok<ServiceResponse<List<string>>>> GetSearchSuggestions(string searchSuggestion, IProductServices productService, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Products Suggestions");

        var result = await productService.SearchSuggestions(searchSuggestion);

        return TypedResults.Ok(result);
    }


    private async static Task<Ok<ServiceResponse<ProductSearchResult>>> GetSearchProducts([AsParameters] RequestSearchQueryParameters parameters, IProductServices productService, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Products by Search Query");

        var result = await productService.SearchProducts(parameters);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<PaginatedList<Product, ProductDTO>>> GetProductQuery([AsParameters] RequestQueryParameters parameters, ApplicationDbContext context, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Products by Query");
        var query = context.Products.AsQueryable();

        query = QueryHelpers.BuildQuery(query, parameters);

        var result = await PaginatedList<Product, ProductDTO>.CreateAsync(query, parameters.Page, parameters.PageSize);

        return TypedResults.Ok(result);
    }


    private async static Task<Ok<ServiceResponse<GetAllResponse>>> GetProductsBy(string categoryUrl, IProductServices productService, ILogger<Program> _logger, ByteFileUtility byteFileUtility)
    {
        _logger.Log(LogLevel.Information, "Get Products by Category");

        var result = await productService.GetProductsBy(categoryUrl);

        return TypedResults.Ok(result);
    }


    private async static Task<FileContentHttpResult> MediaEndpoint(string fileName, string entity, ByteFileUtility byteFileUtility, HttpContext context)
    {
        var filePath = byteFileUtility.GetFileFullPath(fileName, entity);
        byte[] encryptedData = await System.IO.File.ReadAllBytesAsync(filePath);

        //? Decrypt if the file is encrypted
        // var decryptedData = byteFileUtility.DecryptFile(encryptedData);

        context.Response.Headers.Append("Content-Disposition", "inline; filename=preview.jpg");

        return TypedResults.File(encryptedData, "image/jpeg");

    }


    //Write
    private static async Task<Ok<ServiceResponse<ProductCreateDTO>>> CreateProduct(IProductRepository _productRepo, IProductServices productServices,
                  ProductCreateDTO product_C_DTO, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Product");

        // await AccessControl.CheckProductPermissionFlag(context, "product-add");

        var productDTO = await productServices.Create(product_C_DTO);

        return TypedResults.Ok(productDTO);
    }

    private async static Task<Ok<ServiceResponse<bool>>> UpdateProduct(IProductServices productServices, ProductUpdateDTO product_U_DTO, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Product");

        // await AccessControl.CheckProductPermissionFlag(context,"product-update");

        var Success = await productServices.Edit(product_U_DTO);

        return TypedResults.Ok(Success);
    }

    // Read 
    private async static Task<Ok<ServiceResponse<ProductDTO>>> GetProduct(IProductServices productService, IProductRepository _productRepo, ILogger<Program> _logger, Guid id, ByteFileUtility byteFileUtility)
    {
        _logger.Log(LogLevel.Information, "Get Product");

        var result = await productService.GetSingleProductBy(id);

        return TypedResults.Ok(result);
    }
    private async static Task<Ok<ServiceResponse<GetAllResponse>>> GetAllProduct(IProductServices productService, ILogger<Program> _logger, HttpContext context, ByteFileUtility byteFileUtility)
    {
        _logger.Log(LogLevel.Information, "Getting all Products");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await productService.GetAll();

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteProduct(IProductServices productService, IProductRepository _productRepo, Guid id, IUnitOfWork unitOfWork, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Product");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        // await AccessControl.CheckProductPermissionFlag(context, "product-remove");

        var result = await productService.Delete(id);

        return TypedResults.Ok(result);
    }
}
