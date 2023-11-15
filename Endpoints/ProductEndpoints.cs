using System.Net;
using ApiAryanakala.Entities;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Models;
using Microsoft.AspNetCore.Mvc;
using ApiAryanakala.Mapper.Query;
using ApiAryanakala.Mapper.Write;
using ApiAryanakala.Filter;
using ApiAryanakala.Data;
using ApiAryanakala.Utility;
using ApiAryanakala.Models.DTO.ProductDtos;

namespace ApiAryanakala.Endpoints
{
    public static class ProductEndpoints
    {
        public static void ConfigureProductEndpoints(this WebApplication app)
        {
            app.MapGet("/api/products", GetAllProduct)
            .RequireAuthorization()
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
            .WithName("GetProduct").AddEndpointFilter<ValidationFilter<Guid>>()
            .Produces<APIResponse>(200);

            app.MapPost("/api/product", CreateProduct)
            .RequireAuthorization()
            .Produces(401)
            .Produces<APIResponse>(201)
            .Produces(400)
            .AddEndpointFilter<ValidationFilter<ProductCreateDTO>>()
            .ProducesValidationProblem()
            .WithName("CreateProduct")
            .Accepts<ProductCreateDTO>("application/json");

            app.MapPut("/api/product", UpdateProduct)
            .RequireAuthorization()
            .WithName("UpdateProduct")
            .AddEndpointFilter<ValidationFilter<Guid>>()
            .AddEndpointFilter<ValidationFilter<ProductUpdateDTO>>()
            .ProducesValidationProblem()
            .Accepts<ProductUpdateDTO>("application/json")
            .Produces<APIResponse>(200).Produces(400);

            app.MapDelete("/api/product/{id:guid}", DeleteProduct)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<Guid>>()
            .ProducesValidationProblem()
            .Produces(204).Produces(400);
        }

        //Write
        private async static Task<IResult> CreateProduct(IProductRepository _productRepo,
                      [FromBody] ProductCreateDTO product_C_DTO, IUnitOfWork unitOfWork, ILogger<Program> _logger, HttpContext context)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            _logger.Log(LogLevel.Information, "Create Product");

            await AccessControl.CheckProductPermissionFlag(context);

            if (_productRepo.GetAsync(product_C_DTO.Title).GetAwaiter().GetResult() != null)
            {
                response.ErrorMessages.Add("Product Name/Code already Exists");
                return Results.BadRequest(response);
            }

            var product = product_C_DTO.ToProducts();

            await _productRepo.CreateAsync(product);
            await unitOfWork.SaveChangesAsync();

            var productDTO = product.ToCreateResponse();
            response.Result = productDTO;
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.Created;
            return Results.Ok(response);
        }

        private async static Task<IResult> UpdateProduct(IProductRepository _productRepo,
         [FromBody] ProductUpdateDTO product_U_DTO, IUnitOfWork unitOfWork, ILogger<Program> _logger, HttpContext context)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            _logger.Log(LogLevel.Information, "Update Product");

            await AccessControl.CheckProductPermissionFlag(context);

            var product = product_U_DTO.ToProduct();
            await _productRepo.UpdateAsync(product);
            await unitOfWork.SaveChangesAsync();

            response.Result = product.ToUpdateResponse();
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }

        // Read 
        private async static Task<IResult> GetProduct(IProductRepository _productRepo, ILogger<Program> _logger, Guid id)
        {
            APIResponse response = new();
            _logger.Log(LogLevel.Information, "Get Product");

            var product = await _productRepo.GetAsync(id);
            response.Result = product.ToProductResponse();
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }
        private async static Task<IResult> GetAllProduct(IProductRepository _productRepo, ILogger<Program> _logger, HttpContext context)
        {
            APIResponse response = new();
            _logger.Log(LogLevel.Information, "Getting all Products");

            await AccessControl.CheckProductPermissionFlag(context);

            var products = await _productRepo.GetAllAsync();
            response.Result = products.ToProductsResponse();
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }

        private async static Task<IResult> DeleteProduct(IProductRepository _productRepo, Guid id, IUnitOfWork unitOfWork, ILogger<Program> _logger, HttpContext context)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            _logger.Log(LogLevel.Information, "Delete Product");

            await AccessControl.CheckProductPermissionFlag(context);

            Product productFromStore = await _productRepo.GetAsync(id);
            if (productFromStore != null)
            {
                await _productRepo.RemoveAsync(productFromStore);
                await unitOfWork.SaveChangesAsync();
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

}