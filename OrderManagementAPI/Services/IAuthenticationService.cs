using OrderManagementAPI.Dto;

namespace OrderManagementAPI.Services;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
    void Logout();
}