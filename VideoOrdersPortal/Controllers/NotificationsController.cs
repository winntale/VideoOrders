using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VideoOrdersPortal.Controllers;

[Authorize]
public sealed class NotificationsController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();
}
