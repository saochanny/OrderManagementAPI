using System.Net.Mime;
using log4net;
using OrderManagementAPI.Exceptions;
using OrderManagementAPI.Response;
using ValidationException = OrderManagementAPI.Exceptions.ValidationException;

namespace OrderManagementAPI.Middleware;

/// <summary>
/// Middleware to handle all unhandled exceptions globally, similar to Spring Boot's @ControllerAdvice.
/// It logs the exceptions using log4net and returns a consistent <see cref="BaseBodyResponse"/> JSON response.
/// </summary>
public class GlobalExceptionHandler(RequestDelegate next)
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(GlobalExceptionHandler));


    /// <summary>
    /// Invokes the middleware to process the HTTP context.
    /// Catches all unhandled exceptions, logs them, and writes a structured JSON response.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // Log the exception with stack trace
            Log.ErrorFormat("Unhandled exception occurred while processing request to {0} , {1}", context.Request.Path,
                ex);
            await HandleExceptionAsync(context, ex);
        }
    }


    /// <summary>
    /// Handles the exception by mapping it to a proper HTTP status code and returning
    /// a <see cref="BaseBodyResponse"/> in JSON format.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A task that represents the asynchronous operation of writing the response.</returns>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Map exception types to HTTP status codes
        var statusCode = exception switch
        {
            ResourceNotFoundException => StatusCodes.Status404NotFound,
            BadRequestException => StatusCodes.Status400BadRequest,
            ForbiddenException => StatusCodes.Status403Forbidden,
            ApiException apiEx => apiEx.StatusCode,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            AppException appException => appException.StatusCode,
            _ => StatusCodes.Status500InternalServerError
        };


        // Get the error message
        var message = exception switch
        {
            ApiException apiEx => apiEx.Message,
            BadRequestException _ => exception.Message,
            ForbiddenException _ => exception.Message,
            ValidationException validationEx => string.Join("; ", validationEx.Errors),
            UnauthorizedException => exception.Message,
            UnauthorizedAccessException => exception.Message,
            AppException appEx => appEx.Message,
            _ => $"Internal server error : {exception.Message}"
        };

        // Build the standardized response
        var response = BaseBodyResponse.BodyFailed(statusCode, message);

        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsJsonAsync(response);
    }
}