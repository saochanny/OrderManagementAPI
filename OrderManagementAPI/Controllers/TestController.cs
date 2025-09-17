using Microsoft.AspNetCore.Mvc;

namespace OrderManagementAPI.Controllers;

public class TestController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}