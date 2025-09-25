
using OrderManagementAPI.Dto.Request;
using OrderManagementAPI.Dto.Response;
using OrderManagementAPI.Infrastructure.Page;
using OrderManagementAPI.Response;

namespace OrderManagementAPI.Services;

public interface IUserService
{
    Task<UserResponse> RegisterAsync(RegisterUserRequest registerUserRequest);
    
    Task<List<UserResponse>> GetAllAsync();
    
    Task<Page<UserResponse>> GetAllAsPageAsync(PaginationRequest paginationRequest);
    
    Task<UserResponse> GetByIdAsync(int id);

    Task ChangePasswordAsync(int id, ChangePasswordRequest request);

    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest updateRequest);
}