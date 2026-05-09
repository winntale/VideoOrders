using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using VideoOrdersPortal.Auth;

namespace VideoOrdersPortal.Controllers;

public sealed class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string userId, string? returnUrl = null)
    {
        if (!Guid.TryParse(userId, out var parsed))
        {
            ModelState.AddModelError(string.Empty, "UserId должен быть GUID.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, parsed.ToString()) },
            PortalSession.CookieScheme);

        await HttpContext.SignInAsync(
            PortalSession.CookieScheme,
            new ClaimsPrincipal(identity));

        return Redirect(string.IsNullOrEmpty(returnUrl) ? "/Orders" : returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(PortalSession.CookieScheme);
        return RedirectToAction(nameof(Login));
    }
}
