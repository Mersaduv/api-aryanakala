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

public class BannerServices : IBannerServices
{
    private readonly ApplicationDbContext _context;
    private readonly ByteFileUtility _byteFileUtility;
    private readonly IUnitOfWork _unitOfWork;

    public BannerServices(ByteFileUtility byteFileUtility, ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _byteFileUtility = byteFileUtility;
        _context = context;
        _unitOfWork = unitOfWork;
    }


    public async Task<ServiceResponse<BannerDto>> AddBanner(BannerCreateDto bannerCreateDto)
    {
        if (_context.Banners.FirstOrDefaultAsync(x => x.Title == bannerCreateDto.Title).GetAwaiter().GetResult() != null)
        {
            return new ServiceResponse<BannerDto>
            {
                Data = new BannerDto(),
                Success = false,
                Message = "Banner/Title already Exists"
            };
        }
        var banner = new Banner
        {
            Title = bannerCreateDto.Title,
            Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Banner>>(
        [
            bannerCreateDto.Url
        ], nameof(Banner), false).First(),
            CategoryId = bannerCreateDto.CategoryId,
            Uri = bannerCreateDto.Uri ?? string.Empty,
            IsPublic = bannerCreateDto.IsPublic,
            Type = bannerCreateDto.Type
        };
        await _context.Banners.AddAsync(banner);
        await _unitOfWork.SaveChangesAsync();

        var BannerDTO = new BannerDto
        {
            Id = banner.Id,
            Title = banner.Title,
            Uri = banner.Uri,
            Image = _byteFileUtility.GetEncryptedFileActionUrl(
        [
            new EntityImageDto { Placeholder = banner.Image.Placeholder!, ImageUrl = banner.Image.ImageUrl! }
        ], nameof(Banner)).First(),
            CategoryId = banner.CategoryId,
            IsPublic = banner.IsPublic,
            Type = banner.Type
        };

        return new ServiceResponse<BannerDto>
        {
            Data = BannerDTO
        };
    }

    public async Task<ServiceResponse<bool>> DeleteBanner(Guid id)
    {
        var success = false;
        var dbBanner = (await GetBannerBy(id)).Data;
        if (dbBanner != null)
        {
            _context.Banners.Remove(dbBanner);
            await _context.SaveChangesAsync();
            success = true;
        }
        return new ServiceResponse<bool>
        {
            Data = success
        };
    }


    public async Task<ServiceResponse<Banner>> GetBannerBy(Guid id)
    {
        var result = await _context.Banners.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == id);
        return new ServiceResponse<Banner>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<Banner>>> GetBanners()
    {
        var Banners = await _context.Banners.Include(x => x.Image).ToListAsync();
        var serviceResponse = new ServiceResponse<IReadOnlyList<Banner>> { Data = Banners };
        return serviceResponse;
    }

    public async Task<ServiceResponse<BannerDto>> UpdateBanner(BannerUpdateDto bannerUpdateDto)
    {
        var dbBanner = (await GetBannerBy(bannerUpdateDto.Id)).Data;
        if (dbBanner == null)
        {
            return new ServiceResponse<BannerDto>
            {
                Data = null,
                Success = false,
                Message = "Banner not found."
            };
        }

        dbBanner.Title = bannerUpdateDto.Title;
        dbBanner.CategoryId = bannerUpdateDto.CategoryId;
        dbBanner.Uri = bannerUpdateDto.Uri ?? string.Empty;
        dbBanner.IsPublic = bannerUpdateDto.IsPublic;
        dbBanner.Type = bannerUpdateDto.Type;
        dbBanner.LastUpdated = DateTime.UtcNow;
        _context.Update(dbBanner);
        await _unitOfWork.SaveChangesAsync();
        var entityImageDtos = new List<EntityImageDto>
        {
            new EntityImageDto { Id = dbBanner.Image.Id, ImageUrl = dbBanner.Image.ImageUrl!, Placeholder = dbBanner.Image.Placeholder! }
        };

        var banner = new BannerDto
        {
            Id = dbBanner.Id,
            Title = dbBanner.Title,
            Image = _byteFileUtility.GetEncryptedFileActionUrl(entityImageDtos, nameof(Banner)).First(),
            CategoryId = dbBanner.CategoryId,
            Uri = dbBanner.Uri,
            IsPublic = dbBanner.IsPublic,
        };

        return new ServiceResponse<BannerDto>
        {
            Data = banner
        };
    }

    public async Task<ServiceResponse<bool>> UpsertBannerImage(Thumbnails thumbnails, Guid id)
    {
        var dbCategory = await _context.Banners.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        if (dbCategory is null)
        {
            return new ServiceResponse<bool> { Success = false, Message = $"{nameof(Category)} not found." };
        }
        dbCategory.Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Banner>>(thumbnails.Thumbnail!, nameof(Category), false).First();
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }
    public async Task<ServiceResponse<bool>> DeleteBannerImage(string fileName)
    {
        var categoryImageFromStore = _context.BannerImages.FirstOrDefault(p => p.ImageUrl == fileName);
        if (categoryImageFromStore != null)
        {
            _context.BannerImages.Remove(categoryImageFromStore);
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

    public async Task<ServiceResponse<IReadOnlyList<Banner>>> GetBannersByCategory(int categoryId)
    {
        var Banners = await _context.Banners.Where(x => x.CategoryId == categoryId).Include(x => x.Image).ToListAsync();
        var serviceResponse = new ServiceResponse<IReadOnlyList<Banner>> { Data = Banners };
        return serviceResponse;
    }
}