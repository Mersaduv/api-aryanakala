using ApiAryanakala.Data;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Services.Product;

public class DetailsServices : IDetailsServices
{

    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public DetailsServices(ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }


    public async Task<ServiceResponse<Details>> AddDetails(Details details)
    {
        if (GetDetailsBy(details.Id).GetAwaiter().GetResult() != null)
        {
            return new ServiceResponse<Details>
            {
                Data = new Details(),
                Success = false,
                Message = "Details Id already Exists"
            };
        }

        await _context.Details.AddAsync(details);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<Details>
        {
            Data = details,
        };
    }

    public async Task<ServiceResponse<bool>> DeleteDetails(Guid id)
    {
        var success = false;
        var details = (await GetDetailsBy(id)).Data;
        if (details != null)
        {
            _context.Details.Remove(details);
            await _unitOfWork.SaveChangesAsync();
            success = true;
        }
        return new ServiceResponse<bool>
        {
            Data = success
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<Details>>> GetAllDetails()
    {
        var result = await _context.Details.ToListAsync();
        var count = result.Count;
        return new ServiceResponse<IReadOnlyList<Details>>
        {
            Count = count,
            Data = result
        };
    }

    public async Task<ServiceResponse<Details>> GetDetailsBy(Guid id)
    {
        var result = await _context.Details.FirstOrDefaultAsync(x => x.Id == id);
        return new ServiceResponse<Details>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<Details>> GetDetailsByCategory(int id)
    {
        var result = await _context.Details.FirstOrDefaultAsync(x => x.CategoryId == id);
        return new ServiceResponse<Details>
        {
            Data = result
        };
    }


    public async Task<ServiceResponse<Guid?>> UpdateDetails(Details details)
    {
        var dbDetails = await _context.Details.FirstOrDefaultAsync(x => x.Id == details.Id);
        if (dbDetails == null)
        {
            return new ServiceResponse<Guid?>
            {
                Data = null,
                Success = false,
                Message = "Details not found."
            };
        }
        details.LastUpdated = DateTime.UtcNow;
        _context.Update(details);
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<Guid?>
        {
            Data = details.Id,
        };
    }
}