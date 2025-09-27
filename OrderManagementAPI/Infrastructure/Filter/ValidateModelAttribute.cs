using System.Net.Mime;
using Microsoft.AspNetCore.Mvc.Formatters;
using OrderManagementAPI.Response;

namespace OrderManagementAPI.Infrastructure.Filter;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;
        // Collect all error messages
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        var response = BaseBodyResponse.BodyFailed(
            StatusCodes.Status400BadRequest,
            string.Join("; ", errors)
        );

        context.Result = new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status400BadRequest,
            ContentTypes = { MediaTypeNames.Application.Json }
        };
    }
}
