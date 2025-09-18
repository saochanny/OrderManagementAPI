using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Models;
using OrderManagementAPI.Services;

namespace OrderManagementAPI.Controllers;

public class OrderController
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            var created = await _service.CreateAsync(order);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Order order)
        {
            order.Id = id;
            var updated = await _service.UpdateAsync(order);
            return Ok(updated);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? customerId, [FromQuery] DateTime? start, [FromQuery] DateTime? end)
        {
            var orders = await _service.GetOrdersAsync(customerId, start, end);
            return Ok(orders);
        }
    }

}