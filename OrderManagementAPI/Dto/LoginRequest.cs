using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.Dto;

public class LoginRequest
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 50 characters")]
    //[RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
    [DataType(DataType.Password)]
    //[RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&]).+$", 
    //    ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number, and one special character")]
    public string Password { get; set; } = string.Empty;
}