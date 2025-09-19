namespace OrderManagementAPI.Models;

public abstract class AuditableEntity
{
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int CreatedBy { get; set; }
    
    public DateTime? UpdatedDate { get; set; }
    public int? UpdatedBy { get; set; }
}
