using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Dto;

namespace OrderManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState); // Returns validation errors automatically

        // Proceed with user creation...
        return Ok(new { message = "User registered successfully" });
    }
}