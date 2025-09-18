using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Models;
using OrderManagementAPI.Services.Impl;

namespace OrderManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerServiceImpl _service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Customer customer) => Ok(await _service.CreateAsync(customer));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Customer customer)
    {
        customer.Id = id;
        var updated = await _service.UpdateAsync(customer);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SoftDeleteAsync(id);
        return NoContent();
    }
}
