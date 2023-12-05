namespace ApiAryanakala.Entities;

public class RequestQueryParameters
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? Filter { get; set; }
    public int? Category { get; set; }
    public string? Sort { get; set; }
    public string? Search { get; set; }
}