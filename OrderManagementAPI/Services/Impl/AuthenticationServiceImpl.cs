using log4net;
using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Config;
using OrderManagementAPI.Dto;
using OrderManagementAPI.Exceptions;
using OrderManagementAPI.Security;
using OrderManagementAPI.Utilizes;

namespace OrderManagementAPI.Services.Impl;

public class AuthenticationServiceImpl(ApplicationDbContext context,IPasswordEncoder passwordEnder, IConfiguration config) : IAuthenticationService
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(AuthenticationServiceImpl));

    public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
    {
        // Find active user by username or email
        var user = await context.Users.Include(u => u.UserRoles) // Include user roles
            .ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u =>
                u.IsActive == true && (u.Username == loginRequest.Username || u.Email == loginRequest.Username));
        
        if (user == null || !passwordEnder.Verify(loginRequest.Password, user.Password))
        {
            throw new UnauthorizedException("Invalid username or password");
        }

        // Get list of role names
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        // Generate JWT Token
        var jwtGenerate = JwtTokenUtil.GenerateJwtToken(user.Username, roles, config);
        
        // Return login response
        return new LoginResponse
        {
            Token = jwtGenerate.token,
            Expiration = jwtGenerate.expiration,
            Roles = roles,
            Email = user.Email,
            Username = user.Username,
            Id = user.Id,
            FullName = user.FullName,
            TokenType = JwtTokenUtil.TokenType
        };
    }

    public void Logout()
    {
        throw new NotImplementedException();
    }
}