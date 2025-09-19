using OrderManagementAPI.Dto;
using OrderManagementAPI.Dto.Request;
using OrderManagementAPI.Dto.Response;

namespace OrderManagementAPI.Services;

public interface ICustomerService
{
    Task<CustomerResponse> CreateAsync(CustomerRequest customerRequest);

    Task<List<CustomerResponse>> GetAllAsync();

    Task<CustomerResponse> GetByIdAsync(int id);
    
    Task<CustomerResponse> UpdateAsync(int id, CustomerRequest updateRequest);

    Task SoftDeleteAsync(int id);
}