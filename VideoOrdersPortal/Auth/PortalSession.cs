using System.Security.Claims;

namespace VideoOrdersPortal.Auth;

public static class PortalSession
{
    public const string CookieScheme = "PortalCookie";

    public static Guid? GetUserId(this HttpContext httpContext)
    {
        var raw = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var userId) ? userId : null;
    }
}
