
using OrderManagementAPI.Dto.Request;
using OrderManagementAPI.Dto.Response;
namespace OrderManagementAPI.Services;

public interface IOrderService
{

    Task<OrderResponse> CreateAsync(OrderRequest orderRequest);

    Task<OrderResponse> UpdateAsync(int id , OrderRequest orderRequest);
    
    Task<OrderResponse> GetByIdAsync(int id);

    Task<List<OrderResponse>> GetOrdersAsync(int? customerId = null, DateTime? start = null, DateTime? end = null);
}