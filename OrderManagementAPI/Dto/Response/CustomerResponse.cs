using OrderManagementAPI.Models;

namespace OrderManagementAPI.Dto.Response;

public class CustomerResponse : AuditableResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public static CustomerResponse ToCustomerResponse(Customer customer)
    {
        return new CustomerResponse
        {
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Id = customer.Id,
            CreatedBy = customer.CreatedBy,
            CreatedDate = customer.CreatedDate,
            UpdatedBy = customer.UpdatedBy,
            UpdatedDate = customer.UpdatedDate,
        };
    }
}