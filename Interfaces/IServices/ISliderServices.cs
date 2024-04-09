using ApiAryanakala.Entities.Product;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto;

namespace ApiAryanakala.Interfaces.IServices;

public interface ISliderServices
{
    Task<ServiceResponse<SliderDto>> AddSlider(SliderCreateDto slider);
    Task<ServiceResponse<SliderDto>> UpdateSlider(SliderUpdateDto slider);
    Task<ServiceResponse<bool>> DeleteSlider(Guid id);
    Task<ServiceResponse<Slider>> GetSliderBy(Guid id);
    Task<ServiceResponse<IReadOnlyList<Slider>>> GetSliders();
    Task<ServiceResponse<bool>> UpsertSliderImages(Thumbnails thumbnails, Guid id);
    Task<ServiceResponse<bool>> DeleteSliderImages(string fileName);
}
