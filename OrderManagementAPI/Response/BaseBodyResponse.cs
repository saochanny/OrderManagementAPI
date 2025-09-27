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


    // -------------------
    // Factory methods for success/failure responses
    public static IActionResult Success(object? data, string message = "Success")
        => BuildResponse(true, StatusCodes.Status200OK, message, data);

    public static IActionResult SuccessMessage(string message = "Success")
        => BuildResponse(true, StatusCodes.Status200OK, message);

    public static IActionResult PageSuccess<T>(Page<T> page, string message = "Success")
    {
        var response = BuildBaseResponse(true, StatusCodes.Status200OK, message);
        response.Data = page.Content;
        response.Meta = page.GetMetaData();
        return BuildObjectResult(response, StatusCodes.Status200OK);
    }

    public static IActionResult Failed(int statusCode, string message)
        => BuildResponse(false, statusCode, message);

    public static BaseBodyResponse BodyFailed(int statusCode, string message)
        => BuildBaseResponse(false, statusCode, message);
    
    
    // -------------------
    // Internal helper: centralize common initialization
    private static BaseBodyResponse BuildBaseResponse(bool isSuccess, int statusCode, string message)
    {
        return new BaseBodyResponse
        {
            IsSuccess = isSuccess,
            Status = new StatusResponse(statusCode, message),
            TimeStamp = DateTime.UtcNow,
            TraceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString(),
            Path = CurrentPath
        };
    }

    private static IActionResult BuildResponse(bool isSuccess, int statusCode, string message, object? data = null)
    {
        var response = BuildBaseResponse(isSuccess, statusCode, message);
        response.Data = data;
        return BuildObjectResult(response, statusCode);
    }
    
    
    // -------------------
    // Internal helper
    private static ObjectResult BuildObjectResult(BaseBodyResponse response, int statusCode)
        => new(response)
        {
            StatusCode = statusCode,
            ContentTypes = { MediaTypeNames.Application.Json }
        };
}