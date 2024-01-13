namespace ApiAryanakala.Models;

public class AppSettings
{
    public AuthSettings AuthSettings { get; set; } = default!;

    // public StripeSettings StripeSettings { get; set; } = default!;
    public RedisCache? RedisCache { get; set; }
}
