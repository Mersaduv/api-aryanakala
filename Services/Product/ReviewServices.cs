using ApiAryanakala.Data;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.LinQ;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto.Review;
using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Services.Product;

public class ReviewServices : IReviewServices
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthServices _authService;
    private readonly IUnitOfWork _unitOfWork;

    public ReviewServices(ApplicationDbContext context, IAuthServices authService, IUnitOfWork unitOfWork)
    {
        _context = context;
        _authService = authService;
        _unitOfWork = unitOfWork;

    }
    public async Task<ServiceResponse<bool>> CheckProductExist(Guid productId)
    {
        var product = await _context.Products
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

    public async Task<ServiceResponse<bool>> CreateReview(ReviewCreateDTO reviewCreate)
    {
        var isExist = (await CheckProductExist(reviewCreate.ProductId)).Data;
        var userId = _authService.GetUserId();
        if (!isExist)
        {

            return new ServiceResponse<bool>
            {
                Data = false,
                Message = "Product code does not exist."
            };
        }

        if (reviewCreate.Rating < 0 || reviewCreate.Rating > 5)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Message = "Invalid rating value."
            };
        }

        var review = new Review
        {
            UserId = userId,
            ProductId = reviewCreate.ProductId,
            Comment = reviewCreate.Comment,
            Rating = reviewCreate.Rating,
            ImageUrl = reviewCreate.ImageUrl,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        await _context.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> DeleteReview(Guid id)
    {
        var success = false;
        var review = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _unitOfWork.SaveChangesAsync();
            success = true;
        }
        return new ServiceResponse<bool>
        {
            Data = success
        };
    }

    public async Task<ServiceResponse<ReviewDto>> GetReviewBy(Guid id)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);

        var result = new ReviewDto
        {
            Id = review!.Id,
            Title = review.Title,
            Comment = review.Comment,
            ImageUrl = review.ImageUrl,
            Rating = review.Rating,
            Status = review.Status,
            UserId = review.UserId,
            UserName = review.UserName,
            NegativePoints = review.NegativePoints,
            PositivePoints = review.PositivePoints,
            Created = review.Created,
            LastUpdated = review.LastUpdated,
        };
        return new ServiceResponse<ReviewDto>
        {
            Data = result
        };
    }


    public async Task<ServiceResponse<PagingModel<ReviewDto>>> GetProductReviews(GetReviewQuery request)
    {
        var query = _context.Reviews.AsNoTracking()
             .Where(x => x.Product!.Id == request.id)
             .Select(x => new ReviewDto
             {
                 Id = x.Id,
                 Comment = x.Comment,
                 Status = x.Status,
                 NegativePoints = x.NegativePoints,
                 PositivePoints = x.PositivePoints,
                 Rating = x.Rating,
                 ImageUrl = x.ImageUrl,
                 Created = x.Created,
                 LastUpdated = x.LastUpdated,
                 UserId = x.UserId,
                 UserName = x.UserName
             });
        var reviews = new List<ReviewDto>();
        var totalCount = await query.CountAsync();
        if (request.dto.PageSize is not null && request.dto.PageIndex is not null)
        {
            reviews = await query.Page(request.dto.PageIndex.Value, request.dto.PageSize.Value).ToListAsync();
        }
        else
        {
            reviews = await query.ToListAsync();
        }

        return new ServiceResponse<PagingModel<ReviewDto>>
        {
            Data = new PagingModel<ReviewDto>(reviews, totalCount, request.dto.PageIndex.GetValueOrDefault(), request.dto.PageSize.GetValueOrDefault())
        };
    }

    public async Task<ServiceResponse<PagingModel<ReviewDto>>> GetReviews(int? page)
    {
        var query = _context.Reviews.AsNoTracking()
              .Select(x => new ReviewDto
              {
                  Id = x.Id,
                  Comment = x.Comment,
                  Status = x.Status,
                  NegativePoints = x.NegativePoints,
                  PositivePoints = x.PositivePoints,
                  Rating = x.Rating,
                  ImageUrl = x.ImageUrl,
                  Created = x.Created,
                  LastUpdated = x.LastUpdated,
                  UserId = x.UserId,
                  UserName = x.UserName
              });
        var reviews = new List<ReviewDto>();
        var totalCount = await query.CountAsync();
        if (page is not null)
        {
            reviews = await query.Page((int)page, 5).ToListAsync();
        }
        else
        {
            reviews = await query.ToListAsync();
        }

        return new ServiceResponse<PagingModel<ReviewDto>>
        {
            Data = new PagingModel<ReviewDto>(reviews, totalCount, (int)page!, 5)
        };
    }

}