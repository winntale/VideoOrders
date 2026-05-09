using Microsoft.AspNetCore.Mvc;

namespace VideoOrdersPortal.Controllers;

public sealed class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return User.Identity?.IsAuthenticated == true
            ? RedirectToAction("Index", "Orders")
            : RedirectToAction("Login", "Account");
    }
}
