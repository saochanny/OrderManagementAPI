using Microsoft.AspNetCore.Identity;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Security;

public class PasswordEncoder : IPasswordEncoder
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string Encode(string rawPassword)
    {
        // Hash the password
        var tempUser = new User(); // Needed because PasswordHasher requires a TUser
        return _passwordHasher.HashPassword(tempUser, rawPassword);
    }

    public bool Verify(string rawPassword, string encodedPassword)
    {
        var tempUser = new User();
        var result = _passwordHasher.VerifyHashedPassword(tempUser, encodedPassword, rawPassword);
        return result != PasswordVerificationResult.Failed;
    }
}