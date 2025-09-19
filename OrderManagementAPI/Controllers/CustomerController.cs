using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Dto.Request;
using OrderManagementAPI.Response;
using OrderManagementAPI.Services;


namespace OrderManagementAPI.Controllers;

[ApiController]
[Route("api/v1/customers")]
public class CustomersController(ICustomerService customerService) : ControllerBase
{
    [Authorize(Roles = "Admin, Staff")]
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        BaseBodyResponse.Success(await customerService.GetAllAsync(), "Get all customers successfully");
    
    
    [Authorize(Roles = "Admin, Staff")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id) =>
        BaseBodyResponse.Success(await customerService.GetByIdAsync(id), "Get customer by id successfully");

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerRequest customerRequest) =>
        BaseBodyResponse.Success(await customerService.CreateAsync(customerRequest), "Create customer successfully");

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CustomerRequest updateRequest)
    {
        var updated = await customerService.UpdateAsync(id, updateRequest);
        return BaseBodyResponse.Success(updated, "Update customer successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await customerService.SoftDeleteAsync(id);
        return BaseBodyResponse.Success(null, "Delete customer successfully");
    }
}