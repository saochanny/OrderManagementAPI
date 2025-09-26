using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Constants;
using OrderManagementAPI.Dto.Request;
using OrderManagementAPI.Response;
using OrderManagementAPI.Services;

namespace OrderManagementAPI.Controllers;

[Route("api/v1/users")]
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

        return BaseBodyResponse.Success(userResponse, MessageConstant.RegisterUserSuccess);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        // Proceed with user creation...
        var userResponses = await userService.GetAllAsync();

        return BaseBodyResponse.Success(userResponses, "Get users successfully");
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("paged")]
    public async Task<IActionResult> GetUsersAsPaged([FromQuery] PaginationRequest request)
    {
        // Proceed get user as page...
        var userResponses = await userService.GetAllAsPageAsync(request);

        return BaseBodyResponse.PageSuccess(userResponses, "Get users as page is successfully");
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetById(int userId)
    {
        // Proceed with user creation...
        var userResponse = await userService.GetByIdAsync(userId);

        return BaseBodyResponse.Success(userResponse, "Get user by id successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest request)
    {
        // Proceed with user creation...
        var userResponse = await userService.UpdateAsync(id, request);

        return BaseBodyResponse.Success(userResponse, "Update users successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/change-password")]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] ChangePasswordRequest request)
    {
        // Proceed with user creation...
        await userService.ChangePasswordAsync(id, request);

        return BaseBodyResponse.Success(null, "Change password successfully");
    }
}