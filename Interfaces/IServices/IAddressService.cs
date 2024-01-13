using ApiAryanakala.Entities.Product;
using ApiAryanakala.Models;

namespace ApiAryanakala.Interfaces.IServices;

public interface IAddressService
{
    Task<ServiceResponse<Address>> GetAddress();
    Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address);
}