namespace OrderManagementAPI.Dto.Response;

public abstract class AuditableResponse
{
    public int CreatedBy;
    public int? UpdatedBy;
    public DateTime CreatedDate;
    public DateTime? UpdatedDate;
}