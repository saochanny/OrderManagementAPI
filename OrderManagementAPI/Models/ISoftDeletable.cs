namespace OrderManagementAPI.Models;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedDate { get; set; }
    int? DeletedBy { get; set; }
}