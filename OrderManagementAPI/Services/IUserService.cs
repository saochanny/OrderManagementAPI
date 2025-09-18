using OrderManagementAPI.Dto;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Services;

public interface IUserService
{
    Task<UserResponse> RegisterAsync(RegisterUserRequest registerUserRequest);
    
    Task<List<UserResponse>> GetAllAsync();
}