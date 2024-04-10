using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto.Review;

namespace ApiAryanakala.Interfaces.IServices;

public interface IReviewServices
{
    Task<ServiceResponse<bool>> CheckProductExist(Guid productId);
    Task<ServiceResponse<bool>> CreateReview(ReviewCreateDTO reviewCreate);
    Task<ServiceResponse<bool>> DeleteReview(Guid id);
    Task<ServiceResponse<PagingModel<ReviewDto>>> GetProductReviews(GetReviewQuery request);
    Task<ServiceResponse<PagingModel<ReviewDto>>> GetReviews(int? page);
    Task<ServiceResponse<ReviewDto>> GetReviewBy(Guid id);

}