using System.Threading.Tasks;
using ApiAryanakala.Models.DTO;
namespace ApiAryanakala.Interfaces.IServices;

public interface IAuthServices
{
    Task<GenerateNewTokenDTO> GenerateNewToken(GenerateNewTokenDTO command);
    Task Register(LoginRequestDTO command);
    Task<LoginResponseDTO> Login(LoginRequestDTO command);
}