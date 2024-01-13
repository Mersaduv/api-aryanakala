using ApiAryanakala.Const;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;

namespace ApiAryanakala.Endpoints;

public static class AddressEndpoints
{
    public static IEndpointRouteBuilder MapAddressApi(this IEndpointRouteBuilder apiGroup)
    {
        var addressGroup = apiGroup.MapGroup(Constants.Address);
        // .RequireAuthorization();

        addressGroup.MapGet(string.Empty, GetAddress);
        addressGroup.MapPost(string.Empty, AddOrUpdateAddress);

        return apiGroup;
    }

    private static async Task<ServiceResponse<Address?>> GetAddress(IAddressService addressService) =>
        await addressService.GetAddress();

    private static async Task<ServiceResponse<Address>> AddOrUpdateAddress(IAddressService addressService,
        Address address) =>
        await addressService.AddOrUpdateAddress(address);
}