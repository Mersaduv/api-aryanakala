using ApiAryanakala.Entities.Product;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto;

namespace ApiAryanakala.Interfaces.IServices;

public interface IBannerServices
{
    Task<ServiceResponse<BannerDto>> AddBanner(BannerCreateDto Banner);
    Task<ServiceResponse<BannerDto>> UpdateBanner(BannerUpdateDto Banner);
    Task<ServiceResponse<bool>> DeleteBanner(Guid id);
    Task<ServiceResponse<Banner>> GetBannerBy(Guid id);
    Task<ServiceResponse<IReadOnlyList<Banner>>> GetBanners();
    Task<ServiceResponse<IReadOnlyList<Banner>>> GetBannersByCategory(int categoryId);
    Task<ServiceResponse<bool>> UpsertBannerImage(Thumbnails thumbnails, Guid id);
    Task<ServiceResponse<bool>> DeleteBannerImage(string fileName);
}