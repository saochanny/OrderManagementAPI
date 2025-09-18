using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Config;
using OrderManagementAPI.Dto;
using OrderManagementAPI.Exceptions;
using OrderManagementAPI.Models;
using OrderManagementAPI.Security;

namespace OrderManagementAPI.Services.Impl;

public class UserServiceImpl(ApplicationDbContext context, IPasswordEncoder passwordEncoder): IUserService
{
    public async Task<UserResponse> RegisterAsync(RegisterUserRequest registerUserRequest)
    {
        // Check if user exists
        if (await context.Users.AnyAsync(u => u.Username == registerUserRequest.Username || u.Email == registerUserRequest.Email))
        {
            throw new AppException("Username or Email is already taken");
        }
        // Hash password
        var hashedPassword = passwordEncoder.Encode(registerUserRequest.Password);
        
        // Fetch default role (e.g., "User")
        var defaultRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Staff");
        if (defaultRole == null)
        {
            throw new Exception("Default role not found. Please seed roles first.");
        }

        var user = new User
        {
            Email = registerUserRequest.Email,
            FullName = registerUserRequest.FullName,
            IsActive = true,
            Password = hashedPassword,
            CreatedAt = DateTime.Now,
            Phone = registerUserRequest.Phone,
            Username = registerUserRequest.Username,
            UserRoles = new List<UserRole>
            {
                new UserRole { RoleId = defaultRole.Id }
            }
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        // Reload user with roles
        var savedUser = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        // Map to UserResponse
        return MapToUserResponse(savedUser!);
    }

    public async Task<List<UserResponse>> GetAllAsync()
    {
        var users = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync(); // load once

        return users.Select(MapToUserResponse).ToList();
    }

    private UserResponse MapToUserResponse(User user)
    {
        var response = new UserResponse
        {
            Id = user!.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
        return response;
    } 
}