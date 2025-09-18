using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Dto;
using OrderManagementAPI.Response;
using OrderManagementAPI.Services;

namespace OrderManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IAuthenticationService authService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        return !ModelState.IsValid
            ? BaseBodyResponse.Failed(StatusCodes.Status400BadRequest, "Invalid input")
            : BaseBodyResponse.Success(authService.LoginAsync(request));
    }
}