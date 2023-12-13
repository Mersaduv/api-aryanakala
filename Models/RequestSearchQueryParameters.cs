using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace ApiAryanakala.Models;

public class RequestSearchQueryParameters
{
    [FromQuery(Name = "q")]
    public string SearchText { get; set; }
    [FromQuery(Name = "page")]
    public int Page { get; set; }
}