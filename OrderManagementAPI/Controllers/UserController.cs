using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Constants;
using OrderManagementAPI.Dto;
using OrderManagementAPI.Response;
using OrderManagementAPI.Services;

namespace OrderManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState); // Returns validation errors automatically

        // Proceed with user creation...
        var userResponse = await userService.RegisterAsync(request);
        
        return BaseBodyResponse.Success(userResponse , MessageConstant.RegisterUserSuccess);
    }
    
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        // Proceed with user creation...
        var userResponses = await userService.GetAllAsync();
        
        return BaseBodyResponse.Success(userResponses , "Get Users Success");
    }
}