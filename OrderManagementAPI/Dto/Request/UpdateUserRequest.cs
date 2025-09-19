using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.Dto.Request;

public class UpdateUserRequest
{
    [StringLength(255, ErrorMessage = "Full name cannot exceed 255 characters.")]
    public string? FullName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number.")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
    public string? Email { get; set; }

    public List<int>? RoleIds { get; set; }

}
