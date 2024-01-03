using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDtos.Rating;
using static ApiAryanakala.Services.Product.RatingServices;

namespace ApiAryanakala.Interfaces.IServices;

public interface IRatingServices
{
    Task<ServiceResponse<bool>> CheckProductExist(Guid productId);
    Task<ServiceResponse<bool>> CreateRating(RatingCreateDTO ratingCreate);
    Task<ServiceResponse<PagingModel<RatingDto>>> GetRating(GetRatingQuery request);
}