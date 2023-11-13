using System.Net;
using ApiAryanakala.Entities;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto;
using Microsoft.AspNetCore.Mvc;
using ApiAryanakala.Mapper.Query;
using ApiAryanakala.Mapper.Write;
using ApiAryanakala.Filter;

namespace ApiAryanakala.Endpoints
{
    public static class ProductEndpoints
    {
        public static void ConfigureProductEndpoints(this WebApplication app)
        {
            app.MapGet("/api/products", GetAllProduct)
            .WithName("GetProducts").Produces<APIResponse>(200);
            app.MapGet("/api/product/{id:guid}", GetProduct)
            .WithName("GetProduct").AddEndpointFilter<ValidationFilter<Guid>>()
            .Produces<APIResponse>(200);

            app.MapPost("/api/product", CreateProduct)
            .AddEndpointFilter<ValidationFilter<ProductCreateDTO>>()
            .ProducesValidationProblem()
            .WithName("CreateProduct")
            .Accepts<ProductCreateDTO>("application/json")
            .Produces<APIResponse>(201)
            .Produces(400);
            app.MapPut("/api/product", UpdateProduct)
            .WithName("UpdateProduct")
            .AddEndpointFilter<ValidationFilter<Guid>>()
            .AddEndpointFilter<ValidationFilter<ProductUpdateDTO>>()
            .ProducesValidationProblem()
            .Accepts<ProductUpdateDTO>("application/json")
            .Produces<APIResponse>(200).Produces(400);

            app.MapDelete("/api/product/{id:guid}", DeleteProduct)
            .AddEndpointFilter<ValidationFilter<Guid>>()
            .ProducesValidationProblem()
            .Produces(204).Produces(400);
        }

        //Write
        // [Authorize]
        private async static Task<IResult> CreateProduct(IProductRepository _productRepo,
                 [FromBody] ProductCreateDTO product_C_DTO, IUnitOfWork unitOfWork)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

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
            return Results.Ok(response); ;
        }

        private async static Task<IResult> UpdateProduct(IProductRepository _productRepo,
         [FromBody] ProductUpdateDTO product_U_DTO, IUnitOfWork unitOfWork)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

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
        private async static Task<IResult> GetAllProduct(IProductRepository _productRepo, ILogger<Program> _logger)
        {
            APIResponse response = new();
            _logger.Log(LogLevel.Information, "Getting all Products");

            var products = await _productRepo.GetAllAsync();
            response.Result = products.ToProductsResponse();
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }

        private async static Task<IResult> DeleteProduct(IProductRepository _productRepo, Guid id, IUnitOfWork unitOfWork)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };


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