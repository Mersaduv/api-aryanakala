namespace ApiAryanakala.Models;

public class ActionResult<T>
{
    public T Data { get; set; }
    public int Total { get; set; }
    public int PageCount { get; set; }
    public int Size { get; set; }
    public int Page { get; set; }
}