using System.Net;
using ApiAryanakala.Entities;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Models;
using ApiAryanakala.Filter;
using ApiAryanakala.Data;
using ApiAryanakala.Utility;
using ApiAryanakala.Models.DTO.ProductDtos;
using ApiAryanakala.Interfaces.IServices;

namespace ApiAryanakala.Endpoints;

public static class ProductEndpoints
{
    public static void ConfigureProductEndpoints(this WebApplication app)
    {
        app.MapGet("/api/products", GetAllProduct)
        .WithName("GetProducts").Produces<APIResponse>(200);

        app.MapGet("/api/main", async ([AsParameters] RequestQueryParameters parameters, ApplicationDbContext context) =>
        {
        var query = context.Products.AsQueryable();

        query = QueryHelpers.BuildQuery(query, parameters);

        var result = await PaginatedList<Product, ProductDTO>.CreateAsync(query, parameters.Page, parameters.PageSize);
        return result;
        })
        .WithName("GetMain").Produces<APIResponse>(200);

        app.MapGet("/api/product/{id:guid}", GetProduct)
        .WithName("GetProduct").AddEndpointFilter<GuidValidationFilter>()
        .Produces<APIResponse>(200);

        app.MapGet("/api/category/{categoryUrl}", GetProductsBy)
         .WithName("GetProductsByCategory")
         .Produces<APIResponse>(200);

        app.MapGet("/api/base/images/{entity}/{fileName}", MediaEndpoint);

        app.MapGet("/api/search", GetSearchProducts)
        .WithName("SearchProducts").Produces<APIResponse>(200);

        app.MapGet("/api/search-suggestions", GetSearchSuggestions)
        .WithName("SearchSuggestions").Produces<APIResponse>(200);

        app.MapPost("/api/product", CreateProduct)
        .Produces(401)
        .Produces<APIResponse>(201)
        .Produces(400)
        .AddEndpointFilter<ModelValidationFilter<ProductCreateDTO>>()
        .ProducesValidationProblem()
        .WithName("CreateProduct")
        .Accepts<ProductCreateDTO>("multipart/form-data");

        app.MapPut("/api/product", UpdateProduct)
        .WithName("UpdateProduct")
        .AddEndpointFilter<GuidValidationFilter>()
        .AddEndpointFilter<ModelValidationFilter<ProductUpdateDTO>>()
        .ProducesValidationProblem()
        .Produces<APIResponse>(200)
        .Produces(400)
        .Accepts<ProductUpdateDTO>("multipart/form-data");

        app.MapDelete("/api/product/{id:guid}", DeleteProduct)
        .RequireAuthorization()
        .AddEndpointFilter<GuidValidationFilter>()
        .ProducesValidationProblem()
        .Produces(204).Produces(400);
    }

    private static async Task<IResult> GetSearchSuggestions(string searchSuggestion, IProductServices productService, ILogger<Program> _logger)
    {
        APIResponse response = new();
        _logger.Log(LogLevel.Information, "Get Products Suggestions");

        var result = await productService.SearchSuggestions(searchSuggestion);
        response.Result = result;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }


    private async static Task<IResult> GetSearchProducts([AsParameters] RequestSearchQueryParameters parameters, IProductServices productService, ILogger<Program> _logger)
    {
        APIResponse response = new();
        _logger.Log(LogLevel.Information, "Get Products by Search Query");

        var result = await productService.SearchProducts(parameters);
        response.Result = result;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }


    private async static Task<IResult> GetProductsBy(string categoryUrl, IProductServices productService, ILogger<Program> _logger, ByteFileUtility byteFileUtility)
    {
        APIResponse response = new();
        _logger.Log(LogLevel.Information, "Get Products by Category");

        var result = await productService.GetProductsBy(categoryUrl);
        response.Result = result;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }


    private async static Task<IResult> MediaEndpoint(string fileName, string entity, ByteFileUtility byteFileUtility, HttpContext context)
    {
        var filePath = byteFileUtility.GetFileFullPath(fileName, entity);
        byte[] encryptedData = await System.IO.File.ReadAllBytesAsync(filePath);

        //? Decrypt if the file is encrypted
        // var decryptedData = byteFileUtility.DecryptFile(encryptedData);

        context.Response.Headers.Append("Content-Disposition", "inline; filename=preview.jpg");

        return Results.File(encryptedData, "image/jpeg");

    }


    //Write
    private static async Task<IResult> CreateProduct(IProductRepository _productRepo, IProductServices productServices,
                  ProductCreateDTO product_C_DTO, ILogger<Program> _logger, HttpContext context)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
        _logger.Log(LogLevel.Information, "Create Product");

        // await AccessControl.CheckProductPermissionFlag(context, "product-add");

        if (_productRepo.GetAsyncBy(product_C_DTO.Title).GetAwaiter().GetResult() != null)
        {
            response.ErrorMessages.Add("Product Name/Code already Exists");
            return Results.BadRequest(response);
        }

        var productDTO = await productServices.Create(product_C_DTO);

        response.Result = productDTO;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.Created;
        return Results.Ok(response);
    }

    private async static Task<IResult> UpdateProduct(IProductServices productServices, ProductUpdateDTO product_U_DTO, ILogger<Program> _logger)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
        _logger.Log(LogLevel.Information, "Update Product");

        // await AccessControl.CheckProductPermissionFlag(context,"product-update");

        var isSuccess = await productServices.Edit(product_U_DTO);
        response.IsSuccess = isSuccess;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }

    // Read 
    private async static Task<IResult> GetProduct(IProductServices productService, IProductRepository _productRepo, ILogger<Program> _logger, Guid id, ByteFileUtility byteFileUtility)
    {
        APIResponse response = new();
        _logger.Log(LogLevel.Information, "Get Product");

        var result = await productService.GetSingleProductBy(id);
        response.Result = result;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }
    private async static Task<IResult> GetAllProduct(IProductServices productService, ILogger<Program> _logger, HttpContext context, ByteFileUtility byteFileUtility)
    {
        APIResponse response = new();
        _logger.Log(LogLevel.Information, "Getting all Products");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await productService.GetAll();
        response.Result = result;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }

    private async static Task<IResult> DeleteProduct(IProductServices productService, IProductRepository _productRepo, Guid id, IUnitOfWork unitOfWork, ILogger<Program> _logger, HttpContext context)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
        _logger.Log(LogLevel.Information, "Delete Product");

        await AccessControl.CheckProductPermissionFlag(context, "product-remove");

        var result = await productService.Delete(id);
        if (result)
        {
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.NoContent;
            return Results.Ok(response);
        }
        else
        {
            response.ErrorMessages.Add("Invalid Id");
            return Results.BadRequest(response);
        }
    }
}
