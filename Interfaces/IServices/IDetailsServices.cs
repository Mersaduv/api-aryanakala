using ApiAryanakala.Entities.Product;
using ApiAryanakala.Models;

namespace ApiAryanakala.Interfaces.IServices;

public interface IDetailsServices
{
    Task<ServiceResponse<Details>> AddDetails(Details details);
    Task<ServiceResponse<Guid?>> UpdateDetails(Details details);
    Task<ServiceResponse<bool>> DeleteDetails(Guid id);
    Task<ServiceResponse<Details>> GetDetailsBy(Guid id);
    Task<ServiceResponse<Details>> GetDetailsByCategory(int id);
    Task<ServiceResponse<IReadOnlyList<Details>>> GetAllDetails();
}