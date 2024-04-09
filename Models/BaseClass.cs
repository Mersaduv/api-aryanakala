namespace ApiAryanakala.Models;

public class BaseClass<T>
{
    public T? Id { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdated { get; set; } = DateTime.UtcNow;
}
