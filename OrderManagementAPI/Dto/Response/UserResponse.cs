using OrderManagementAPI.Models;

namespace OrderManagementAPI.Dto.Response;

public class UserResponse
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<string> Roles { get; set; } = null!;

    public static UserResponse ToUserResponse(User? user)
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
            Roles = user.UserRoles.Where(ur => ur.Role != null).Select(ur => ur.Role!.Name).ToList()
        };
        return response;
    }
}