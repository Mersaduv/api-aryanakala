namespace ApiAryanakala.Entities.User.Security;

public class Role
{
    public int Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}