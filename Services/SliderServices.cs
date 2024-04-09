using System;
using ApiAryanakala.Data;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO;
using ApiAryanakala.Models.DTO.ProductDto;
using ApiAryanakala.Utility;
using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Services;

public class SliderServices : ISliderServices
{
    private readonly ApplicationDbContext _context;
    private readonly ByteFileUtility _byteFileUtility;
    private readonly IUnitOfWork _unitOfWork;

    public SliderServices(ByteFileUtility byteFileUtility, ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _byteFileUtility = byteFileUtility;
        _context = context;
        _unitOfWork = unitOfWork;
    }


    public async Task<ServiceResponse<SliderDto>> AddSlider(SliderCreateDto sliderCreateDto)
    {
        if (_context.Sliders.FirstOrDefaultAsync(x => x.Title == sliderCreateDto.Title).GetAwaiter().GetResult() != null)
        {
            return new ServiceResponse<SliderDto>
            {
                Data = new SliderDto(),
                Success = false,
                Message = "Slider/Title already Exists"
            };
        }
        var slider = new Slider
        {
            Title = sliderCreateDto.Title,
            Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Slider>>(
        [
            sliderCreateDto.Url
        ], nameof(Slider), false).First(),
            CategoryId = sliderCreateDto.CategoryId,
            Uri = sliderCreateDto.Uri,
            IsPublic = sliderCreateDto.IsPublic
        };
        await _context.Sliders.AddAsync(slider);
        await _unitOfWork.SaveChangesAsync();

        var sliderDTO = new SliderDto
        {
            Id = slider.Id,
            Title = slider.Title,
            Uri = slider.Uri,
            Image = _byteFileUtility.GetEncryptedFileActionUrl(
        [
            new EntityImageDto { Placeholder = slider.Image.Placeholder!, ImageUrl = slider.Image.ImageUrl! }
        ], nameof(Slider)).First(),
            CategoryId = slider.CategoryId,
            IsPublic = slider.IsPublic,
            // Omit ChildCategories here to avoid circular reference
        };

        return new ServiceResponse<SliderDto>
        {
            Data = sliderDTO
        };
    }

    public async Task<ServiceResponse<bool>> DeleteSlider(Guid id)
    {
        var success = false;
        var dbSlider = (await GetSliderBy(id)).Data;
        if (dbSlider != null)
        {
            _context.Sliders.Remove(dbSlider);
            await _context.SaveChangesAsync();
            success = true;
        }
        return new ServiceResponse<bool>
        {
            Data = success
        };
    }


    public async Task<ServiceResponse<Slider>> GetSliderBy(Guid id)
    {
        var result = await _context.Sliders.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == id);
        return new ServiceResponse<Slider>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<Slider>>> GetSliders()
    {
        var sliders = await _context.Sliders.Include(x => x.Image).ToListAsync();
        var serviceResponse = new ServiceResponse<IReadOnlyList<Slider>> { Data = sliders };
        return serviceResponse;
    }

    public async Task<ServiceResponse<SliderDto>> UpdateSlider(SliderUpdateDto sliderDto)
    {
        var dbSlider = (await GetSliderBy(sliderDto.Id)).Data;
        if (dbSlider == null)
        {
            return new ServiceResponse<SliderDto>
            {
                Data = null,
                Success = false,
                Message = "Slider not found."
            };
        }

        dbSlider.Title = sliderDto.Title;
        dbSlider.CategoryId = sliderDto.CategoryId;
        dbSlider.Uri = sliderDto.Uri;
        dbSlider.IsPublic = sliderDto.IsPublic;
        dbSlider.LastUpdated = DateTime.UtcNow;

        _context.Update(dbSlider);
        await _unitOfWork.SaveChangesAsync();
        var entityImageDtos = new List<EntityImageDto>
        {
            new EntityImageDto { Id = dbSlider.Image!.Id, ImageUrl = dbSlider.Image.ImageUrl!, Placeholder = dbSlider.Image.Placeholder! }
        };

        var slider = new SliderDto
        {
            Id = dbSlider.Id,
            Title = dbSlider.Title,
            Image = _byteFileUtility.GetEncryptedFileActionUrl(entityImageDtos, nameof(Slider)).First(),
            CategoryId = dbSlider.CategoryId,
            Uri = dbSlider.Uri,
            IsPublic = dbSlider.IsPublic,
        };

        return new ServiceResponse<SliderDto>
        {
            Data = slider
        };
    }

    public async Task<ServiceResponse<bool>> UpsertSliderImages(Thumbnails thumbnails, Guid id)
    {
        var dbCategory = await _context.Sliders.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        if (dbCategory is null)
        {
            return new ServiceResponse<bool> { Success = false, Message = $"{nameof(Category)} not found." };
        }
        dbCategory.Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Slider>>(thumbnails.Thumbnail!, nameof(Category), false).First();
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }
    public async Task<ServiceResponse<bool>> DeleteSliderImages(string fileName)
    {
        var categoryImageFromStore = _context.SliderImages.FirstOrDefault(p => p.ImageUrl == fileName);
        if (categoryImageFromStore != null)
        {
            _context.SliderImages.Remove(categoryImageFromStore);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Data = false,
                Message = "Image not found."
            };
        }
        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }
}
