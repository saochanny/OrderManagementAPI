
using OrderManagementAPI.Dto.Request;
using OrderManagementAPI.Dto.Response;

namespace OrderManagementAPI.Services;

public interface IUserService
{
    Task<UserResponse> RegisterAsync(RegisterUserRequest registerUserRequest);
    
    Task<List<UserResponse>> GetAllAsync();
    
    Task<UserResponse> GetByIdAsync(int id);

    Task ChangePasswordAsync(int id, ChangePasswordRequest request);

    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest updateRequest);
}