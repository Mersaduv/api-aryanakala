using ApiAryanakala.Entities.Exceptions;

namespace ApiAryanakala.Models;

public class ServiceResponse<T>
{
    public int Count { get; set; }
    public T? Data { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
}