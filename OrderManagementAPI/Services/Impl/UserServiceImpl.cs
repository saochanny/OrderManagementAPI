using log4net;
using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Config;
using OrderManagementAPI.Constants;
using OrderManagementAPI.Dto.Request;
using OrderManagementAPI.Dto.Response;
using OrderManagementAPI.Exceptions;
using OrderManagementAPI.Models;
using OrderManagementAPI.Security;

namespace OrderManagementAPI.Services.Impl;

public class UserServiceImpl(ApplicationDbContext context, IPasswordEncoder passwordEncoder) : IUserService
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(UserServiceImpl));

    public async Task<UserResponse> RegisterAsync(RegisterUserRequest registerUserRequest)
    {
        // Check if user exists
        if (await context.Users.AnyAsync(u =>
                u.Username == registerUserRequest.Username || u.Email == registerUserRequest.Email))
        {
            Log.Warn("User with username already exists.");
            throw new AppException("Username or Email is already taken");
        }

        // Hash password
        var hashedPassword = passwordEncoder.Encode(registerUserRequest.Password);

        // Fetch default role (e.g., "User")
        var defaultRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == nameof(RoleEnum.Staff));
        if (defaultRole == null)
        {
            Log.Warn("No role exists for this user.");
            throw new AppException("Default role not found. Please seed roles first.");
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
                new() { RoleId = defaultRole.Id }
            }
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        Log.Info("Save change user");
        
        // Reload user with roles
        var savedUser = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        // Map to UserResponse
        return UserResponse.ToUserResponse(savedUser!);
    }

    public async Task<List<UserResponse>> GetAllAsync()
    {
        Log.Info("Get all users");
        var users = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync(); // load once

        return users.Select(UserResponse.ToUserResponse).ToList();
    }

    public async Task<UserResponse> GetByIdAsync(int id)
    {
        Log.InfoFormat("Get user with id {0}", id);

        var user = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id); // find by id with include for get reference
        
        return user == null ? throw new ResourceNotFoundException("User", id) : UserResponse.ToUserResponse(user);
    }
    
    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest updateRequest)
    {
        Log.InfoFormat("Update user with id {0}", id);

        var user = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new ResourceNotFoundException("User", id);

        // Update allowed fields
        user.FullName = updateRequest.FullName ?? user.FullName;
        user.Phone = updateRequest.Phone ?? user.Phone;
        user.Email = updateRequest.Email ?? user.Email;
        user.UpdatedAt = DateTime.UtcNow;

        // update roles
        if (updateRequest.RoleIds != null && updateRequest.RoleIds.Any())
        {
            // Clear existing roles
            user.UserRoles.Clear();

            foreach (var roleId in updateRequest.RoleIds)
            {
                user.UserRoles.Add(new UserRole { RoleId = roleId, UserId = user.Id });
            }
        }

        await context.SaveChangesAsync();
        
        // Reload user with roles
        user = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        return UserResponse.ToUserResponse(user!);
    }

    public async Task ChangePasswordAsync(int id, ChangePasswordRequest request)
    {
        Log.InfoFormat("Change password for user with id {0}", id);

        var user = await context.Users.FindAsync(id);
        if (user == null)
            throw new ResourceNotFoundException("User", id);

        // Verify old password
        if (!passwordEncoder.Verify(request.OldPassword, user.Password))
            throw new AppException("Old password is incorrect");

        // Set new password
        user.Password = passwordEncoder.Encode(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        Log.Info("Save change password");
    }
}