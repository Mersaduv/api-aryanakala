using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace ApiAryanakala.Models.RequestQuery;

public class RequestSearchQuery
{
    [FromQuery(Name = "q")]
    public string SearchText { get; set; } = string.Empty;
    [FromQuery(Name = "page")]
    public int Page { get; set; }
}