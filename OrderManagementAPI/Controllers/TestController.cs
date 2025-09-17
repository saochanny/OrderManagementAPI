using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Exceptions;
using OrderManagementAPI.Response;

namespace OrderManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("test")]
    public IActionResult Index()
    {
        var a = 10;
        var b = 0;
        return BaseBodyResponse.Success(a / b, "Sucess");
    }
}