namespace ApiAryanakala.Models.DTO;

public class LoginResponseDTO
{
    public string UserName { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public int ExpireTime { get; set; }
}
