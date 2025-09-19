using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Constants;
using OrderManagementAPI.Dto.Request;
using OrderManagementAPI.Response;
using OrderManagementAPI.Services;

namespace OrderManagementAPI.Controllers;

[Route("api/v1/auth")]
[ApiController]
public class AuthenticationController(IAuthenticationService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        return !ModelState.IsValid
            ? BaseBodyResponse.Failed(StatusCodes.Status400BadRequest, "Invalid input")
            : BaseBodyResponse.Success(await authService.LoginAsync(request), MessageConstant.LoginSuccess);
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> FetchMe()
    {
         return BaseBodyResponse.Success(await authService.FetchMe(), "Fetch user information");
    }
}