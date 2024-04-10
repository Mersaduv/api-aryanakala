using ApiAryanakala.Entities.User;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO;
namespace ApiAryanakala.Interfaces.IServices;

public interface IAuthServices
{
    Task<ServiceResponse<GenerateNewTokenDTO>> GenerateNewToken(GenerateNewTokenDTO command);
    Task<ServiceResponse<Guid>> RegisterAsync(User user, string password);
    Task<ServiceResponse<LoginResponse>> LogInAsync(string email, string password);
    Task<ServiceResponse<bool>> ChangePasswordAsync(Guid userId, string newPassword);
    Task<bool> UserExists(string email);
    Guid GetUserId();
    string GetUserEmail();
    Task<User?> GetUserByEmail(string email);
}