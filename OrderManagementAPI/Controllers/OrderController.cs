using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Dto.Request;
using OrderManagementAPI.Response;
using OrderManagementAPI.Services;

namespace OrderManagementAPI.Controllers;

[ApiController]
[Route("api/v1/orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [Authorize(Roles = "Admin, Staff")]
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest order)
    {
        var created = await orderService.CreateAsync(order);
        return BaseBodyResponse.Success(created, "Order created");
    }

    [Authorize(Roles = "Admin, Staff")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateOrder([FromRoute] int id, OrderRequest order)
    {
        var updated = await orderService.UpdateAsync(id, order);
        return BaseBodyResponse.Success(updated, "Order updated");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? customerId, [FromQuery] DateTime? start,
        [FromQuery] DateTime? end)
    {
        var orders = await orderService.GetOrdersAsync(customerId, start, end);
        return BaseBodyResponse.Success(orders, "Orders retrieved");
    }

    [Authorize]
    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetOrderById([FromRoute] int orderId)
    {
        var orders = await orderService.GetByIdAsync(orderId);
        return BaseBodyResponse.Success(orders, "Order retrieved");
    }
}