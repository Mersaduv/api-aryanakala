namespace ApiAryanakala.Entities.User.Security;

public class UserChangePassword
{
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}