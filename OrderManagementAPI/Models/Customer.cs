using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OrderManagementAPI.Models;

[Index(nameof(Email), IsUnique = true)]
public class Customer
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    public bool IsDeleted { get; set; } = false;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
