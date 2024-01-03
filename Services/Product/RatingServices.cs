using ApiAryanakala.Data;
using ApiAryanakala.Entities;
using ApiAryanakala.Entities.Exceptions;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.LinQ;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDtos.Rating;
using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Services.Product;

public class RatingServices : IRatingServices
{
    private readonly ApplicationDbContext _mainDbContext;


    public RatingServices(ApplicationDbContext mainDbContext)
    {
        _mainDbContext = mainDbContext;

    }
    public async Task<ServiceResponse<bool>> CheckProductExist(Guid productId)
    {
        var product = await _mainDbContext.Products
                   .AsNoTracking().FirstOrDefaultAsync(i => i.Id == productId);
        if (product is null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
            };
        }
        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }

    public async Task<ServiceResponse<bool>> CreateRating(RatingCreateDTO ratingCreate)
    {
        var isExist = (await CheckProductExist(ratingCreate.ProductId)).Data;

        if (!isExist)
        {

            return new ServiceResponse<bool>
            {
                Data = false,
                Message = "Product code does not exist."
            };
        }

        if (ratingCreate.Rate < 0 || ratingCreate.Rate > 5)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Message = "Invalid rating value."
            };
        }


        var rating = new Rating() { Comment = ratingCreate.Comment, Rate = ratingCreate.Rate, ImageUrl = ratingCreate.ImageUrl, UserName = ratingCreate.UserName, ProductId = ratingCreate.ProductId, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow };
        _mainDbContext.Ratings.Add(rating);
        await _mainDbContext.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<PagingModel<RatingDto>>> GetRating(GetRatingQuery request)
    {
        var query = _mainDbContext.Ratings.AsNoTracking()
             .Where(x => x.Product.Slug == request.slug)
             .Select(x => new RatingDto
             {
                 Comment = x.Comment,
                 Rate = x.Rate,
                 ImageUrl = x.ImageUrl,
                 CreateAt = x.CreatedAt,
                 UserName = x.UserName
             });
        var ratings = new List<RatingDto>();
        var totalCount = await query.CountAsync();
        if (request.dto.PageSize is not null && request.dto.PageIndex is not null)
        {
            ratings = await query.Page(request.dto.PageIndex.Value, request.dto.PageSize.Value).ToListAsync();
        }
        else
        {
            ratings = await query.ToListAsync();
        }

        return new ServiceResponse<PagingModel<RatingDto>>
        {
            Data = new PagingModel<RatingDto>(ratings, totalCount, request.dto.PageIndex.GetValueOrDefault(), request.dto.PageSize.GetValueOrDefault())
        };
    }

}