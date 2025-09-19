using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.Dto.Request;

public class CustomerRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name must not exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(150, ErrorMessage = "Email must not exceed 150 characters")]
    public string Email { get; set; } = string.Empty;
    
    
    
    [Required(ErrorMessage = "Phone is required")]
    [StringLength(20, ErrorMessage = "Phone must not exceed 20 characters")]
    public string Phone { get; set; } = string.Empty;
    
}