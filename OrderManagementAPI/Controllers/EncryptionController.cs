using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Response;
using OrderManagementAPI.Utilizes;

namespace OrderManagementAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EncryptionController: ControllerBase
{
    [HttpPost("encrypt")]
    public Task<IActionResult> Encrypt([FromBody] string data)
    {
        return Task.FromResult(BaseBodyResponse.Success(AesEncryptionUtil.Encrypt(data, "12345678901234567890123456789012"), "Success"));
    }
}