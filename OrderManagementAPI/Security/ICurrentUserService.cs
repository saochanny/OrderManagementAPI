namespace OrderManagementAPI.Security;


public interface ICurrentUserService
{
    int UserId { get; } // Current user id from JWT
    string Username { get; } // Current username
    string Email { get; } // Current user email
}