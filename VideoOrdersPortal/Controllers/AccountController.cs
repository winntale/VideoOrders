using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using VideoOrdersPortal.Auth;
using VideoOrdersPortal.Clients;

namespace VideoOrdersPortal.Controllers;

public sealed class AccountController(UserServiceClient users) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string login, string password, string? returnUrl, CancellationToken ct)
    {
        var result = await users.LoginAsync(login, password, ct);
        if (result.User is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Не удалось войти.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        await SignInAsync(result.User);
        return Redirect(string.IsNullOrEmpty(returnUrl) ? "/Orders" : returnUrl);
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string login, string password, CancellationToken ct)
    {
        var result = await users.RegisterAsync(login, password, ct);
        if (result.User is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Не удалось зарегистрироваться.");
            return View();
        }

        await SignInAsync(result.User);
        return Redirect("/Orders");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(PortalSession.CookieScheme);
        return RedirectToAction(nameof(Login));
    }

    private Task SignInAsync(AuthenticatedUser user)
    {
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Login)
            },
            PortalSession.CookieScheme);

        return HttpContext.SignInAsync(PortalSession.CookieScheme, new ClaimsPrincipal(identity));
    }
}
