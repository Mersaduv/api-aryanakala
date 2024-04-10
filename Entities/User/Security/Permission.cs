namespace ApiAryanakala.Entities.User.Security;

public class Permission
{
    public int Id { get; set; }
    public string PermissionFlag { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}