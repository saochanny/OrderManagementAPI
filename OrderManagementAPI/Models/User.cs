using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; }

    [Required]
    [MaxLength(500)]
    public string Password { get; set; }

    
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }

    [MaxLength(15)]
    public string Phone { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
    
    // Navigation property
    public ICollection<UserRole> UserRoles { get; set; }  = new List<UserRole>();
}