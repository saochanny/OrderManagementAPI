
using OrderManagementAPI.Dto.Request;
using OrderManagementAPI.Dto.Response;

namespace OrderManagementAPI.Services;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
    void Logout();
    Task<UserResponse> FetchMe();
}