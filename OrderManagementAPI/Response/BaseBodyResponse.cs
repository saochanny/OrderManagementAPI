using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Infrastructure.Page;

namespace OrderManagementAPI.Response;

public class BaseBodyResponse
{
    private static IHttpContextAccessor? _httpContextAccessor;

    // Configure once at startup
    public static void Configure(IHttpContextAccessor accessor)
    {
        _httpContextAccessor = accessor;
    }

    // Config request path from global
    private static string? CurrentPath =>
        _httpContextAccessor?.HttpContext?.Request?.Path.ToString();
    
    
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsSuccess { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Data { get; set; } // Payload of type T (object, list , primitive , etc,)

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public StatusResponse? Status { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MetaData? Meta { get; set; } // Optional metadata (Usually for Pagination)

    public DateTime TimeStamp { get; set; }

    public string? TraceId { get; set; }

    public string? Path { get; set; }


    // Success with data
    public static IActionResult Success(object? data, string message = "Success")
    {
        var response = new BaseBodyResponse
        {
            IsSuccess = true,
            Status = new StatusResponse(StatusCodes.Status200OK, message),
            Data = data,
            TimeStamp = DateTime.UtcNow,
            TraceId = Activity.Current?.TraceId.ToString(),
            Path = CurrentPath
        };

        return new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status200OK,
            ContentTypes = { MediaTypeNames.Application.Json }
        };
    }


    // Success with page
    public static IActionResult PageSuccess<T>(Page<T> page, string message = "Success")
    {
        var response = new BaseBodyResponse
        {
            IsSuccess = true,
            Status = new StatusResponse(StatusCodes.Status200OK, message),
            Data = page.Content,
            Meta = page.GetMetaData(),
            TimeStamp = DateTime.UtcNow,
            TraceId = Activity.Current?.TraceId.ToString(),
            Path = CurrentPath
        };

        return new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status200OK,
            ContentTypes = { MediaTypeNames.Application.Json }
        };
    }

    // Success with message only
    public static IActionResult SuccessMessage(string message = "Success")
    {
        var response = new BaseBodyResponse
        {
            IsSuccess = true,
            Status = new StatusResponse(StatusCodes.Status200OK, message),
            TimeStamp = DateTime.UtcNow,
            TraceId = Activity.Current?.TraceId.ToString(),
            Path = CurrentPath
        };

        return new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status200OK,
            ContentTypes = { MediaTypeNames.Application.Json }
        };
    }

    // Failed
    public static IActionResult Failed(int statusCode, string message)
    {
        var response = new BaseBodyResponse
        {
            IsSuccess = false,
            Status = new StatusResponse(statusCode, message),
            TimeStamp = DateTime.UtcNow,
            TraceId = Activity.Current?.TraceId.ToString(),
            Path = CurrentPath
        };

        return new ObjectResult(response)
        {
            StatusCode = statusCode,
            ContentTypes = { MediaTypeNames.Application.Json }
        };
    }

    // Body failed (returns object, not IActionResult)
    public static BaseBodyResponse BodyFailed(int statusCode, string message)
    {
        return new BaseBodyResponse
        {
            IsSuccess = false,
            Status = new StatusResponse(statusCode, message),
            TimeStamp = DateTime.UtcNow,
            TraceId = Activity.Current?.TraceId.ToString(),
            Path = CurrentPath
        };
    }
}