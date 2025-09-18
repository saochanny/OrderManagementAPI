using OrderManagementAPI.Models;

namespace OrderManagementAPI.Dto;

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<string> Roles { get; set; }
}