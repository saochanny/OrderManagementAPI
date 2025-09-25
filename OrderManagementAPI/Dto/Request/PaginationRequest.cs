using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Infrastructure.Page;

namespace OrderManagementAPI.Dto.Request;

public class PaginationRequest
{
    [FromQuery(Name = "page")]
    [DefaultValue(1)]
    public int Page { get; set; } = 1; // 1-based, like Spring

    [FromQuery(Name = "size")]
    [DefaultValue(10)]
    public int Size { get; set; } = 10;

    [DefaultValue("Id")]
    [FromQuery(Name = "sortBy")]
    public string? SortBy { get; set; } = "Id";
    
    public string? Filter { get; set; }

    [FromQuery(Name = "ascending")]
    public bool Ascending { get; set; } = true;

    public Pageable ToPageable()
    {
        // Convert to 0-based PageNumber for internal use
        return new Pageable(Page - 1, Size, SortBy, Ascending);
    }
}
