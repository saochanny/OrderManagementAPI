using System.ComponentModel.DataAnnotations;


namespace OrderManagementAPI.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        
        [Required] [MaxLength(50)] public string Name { get; set; } = null!;

        public ICollection<UserRole> UserRoles { get; set; } = null!;
    }
}