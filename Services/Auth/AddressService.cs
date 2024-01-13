using ApiAryanakala.Data;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Services.Auth;

public class AddressService : IAddressService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthServices _authService;
    private readonly IUnitOfWork _unitOfWork;


    public AddressService(ApplicationDbContext context, IAuthServices authService, IUnitOfWork unitOfWork)
    {
        _context = context;
        _authService = authService;
        _unitOfWork = unitOfWork;

    }

    public async Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address)
    {
        var response = new ServiceResponse<Address>();
        var dbAddress = (await GetAddress()).Data;
        if (dbAddress == null)
        {
            address.UserId = _authService.GetUserId();
            _context.Addresses.Add(address);
            response.Data = address;
        }
        else
        {
            dbAddress.FirstName = address.FirstName;
            dbAddress.LastName = address.LastName;
            dbAddress.State = address.State;
            dbAddress.Country = address.Country;
            dbAddress.City = address.City;
            dbAddress.Zip = address.Zip;
            dbAddress.Street = address.Street;
            response.Data = dbAddress;
        }

        await _unitOfWork.SaveChangesAsync();

        return response;
    }

    public async Task<ServiceResponse<Address>> GetAddress()
    {
        Guid userId = _authService.GetUserId();
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.UserId == userId);
        return new ServiceResponse<Address> { Data = address };
    }
}