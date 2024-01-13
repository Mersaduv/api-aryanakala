namespace ApiAryanakala.Entities.User;

public class UserRefreshToken
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    //unique
    public string RefreshToken { get; set; }
    public int RefreshTokenTimeout { get; set; }
    public DateTime CreateDate { get; set; }
    public bool IsValid { get; set; }

}