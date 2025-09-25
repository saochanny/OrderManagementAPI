using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace OrderManagementAPI.Response;

public class BaseBodyResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsSuccess { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BodyResponse? Body { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public StatusResponse? Status { get; set; }
    
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MetaData? Meta { get; set; }
    
    public DateTime TimeStamp { get; set; }
    
    public string? TraceId { get; set; }
    
    
    // Success with data
    public static IActionResult Success(object? data, string message = "Success")
    {
        var response = new BaseBodyResponse
        {
            IsSuccess = true,
            Status = new StatusResponse(StatusCodes.Status200OK, message),
            Body = new BodyResponse { Data = data },
            TimeStamp = DateTime.UtcNow,
            TraceId = Activity.Current?.TraceId.ToString()
        };

        return new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status200OK,
            ContentTypes = { "application/json" }
        };
    }
    
    
    // Success with page
    public static IActionResult PageSuccess <T> (Page<T> page, string message = "Success")
    {
        var response = new BaseBodyResponse
        {
            IsSuccess = true,
            Status = new StatusResponse(StatusCodes.Status200OK, message),
            Body = new BodyResponse { Meta = page.GetMetaData() , Data = page.Content},
            TimeStamp = DateTime.UtcNow,
            TraceId = Activity.Current?.TraceId.ToString()
        };

        return new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status200OK,
            ContentTypes = { "application/json" }
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
            TraceId = Activity.Current?.TraceId.ToString()
        };

        return new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status200OK
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
            TraceId = Activity.Current?.TraceId.ToString()
        };

        return new ObjectResult(response)
        {
            StatusCode = statusCode
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
            TraceId = Activity.Current?.TraceId.ToString()
        };
    }
}