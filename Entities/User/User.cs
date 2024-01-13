namespace ApiAryanakala.Entities.User;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public byte[] Password { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    public virtual Address Address { get; set; } = default!;
    public DateTime RegisterDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
}