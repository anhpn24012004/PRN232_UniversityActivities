using Microsoft.AspNetCore.Mvc;

namespace WebApp_Client.Mvc.Controllers;

public abstract class BaseClientController : Controller
{
    protected bool IsLoggedIn => !string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionKeys.Token));

    protected bool HasRole(string role)
    {
        var roles = HttpContext.Session.GetString(SessionKeys.Roles) ?? string.Empty;
        return roles
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
    }

    protected IActionResult RequireRole(string role)
    {
        if (!IsLoggedIn)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!HasRole(role))
        {
            return RedirectToAction("AccessDenied", "Auth");
        }

        return new EmptyResult();
    }

    protected IActionResult HandleApiFailure(int statusCode, string message)
    {
        if (statusCode == 401)
        {
            TempData["Error"] = message;
            return RedirectToAction("Login", "Auth");
        }

        if (statusCode == 403)
        {
            TempData["Error"] = message;
            return RedirectToAction("AccessDenied", "Auth");
        }

        TempData["Error"] = message;
        return RedirectToAction("Index", "Home");
    }
}
