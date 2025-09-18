using System.Net.Mime;
using log4net;
using OrderManagementAPI.Response;

namespace OrderManagementAPI.Middleware;

public class AuthenticationExceptionHandler(RequestDelegate next)
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(GlobalExceptionHandler));
    public async Task Invoke(HttpContext context)
    {
        await next(context);

        if (context.Response.HasStarted) return;

        switch (context.Response.StatusCode)
        {
            case StatusCodes.Status401Unauthorized:
            {
                Log.Error("(Unauthorized) Token is missing or invalid.");
                var response = BaseBodyResponse.BodyFailed(401, "Token is missing or invalid.");
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsJsonAsync(response);
                break;
            }
            case StatusCodes.Status403Forbidden:
            {
                Log.Error("(Forbidden) You do not have permission to access this resource.");
                var response = BaseBodyResponse.BodyFailed(403, "You do not have permission to access this resource.");
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsJsonAsync(response);
                break;
            }
        }
    }
}