namespace ApiAryanakala.Entities.User.Security;

public class UserRole
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public virtual Role? Role { get; set; }
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}