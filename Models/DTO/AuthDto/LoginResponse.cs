namespace ApiAryanakala.Models.DTO
{
    public class LoginResponse
    {
        public string UserName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int RefreshTokenExpireTime { get; set; }
        public int ExpireTime { get; set; }
    }
}

