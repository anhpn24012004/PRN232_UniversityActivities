using Microsoft.AspNetCore.Mvc;
using WebApp_Client.Mvc.Models.Auth;

namespace WebApp_Client.Mvc.Controllers;

public class AuthController : BaseClientController
{
    private readonly AuthClientService _authClientService;

    public AuthController(AuthClientService authClientService)
    {
        _authClientService = authClientService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _authClientService.LoginAsync(request);
        if (!result.Success || result.Data is null)
        {
            ViewBag.Error = result.Message;
            return View(request);
        }

        var roles = result.Data.Roles ?? new List<string>();
        if (!roles.Contains("Organizer") && !roles.Contains("Staff"))
        {
            ViewBag.Error = "This client is only for Organizer and Staff accounts.";
            return View(request);
        }

        HttpContext.Session.SetString(SessionKeys.Token, result.Data.Token);
        HttpContext.Session.SetString(SessionKeys.Email, result.Data.Email);
        HttpContext.Session.SetString(SessionKeys.FullName, result.Data.FullName);
        HttpContext.Session.SetString(SessionKeys.Roles, string.Join(",", roles));

        if (roles.Contains("Organizer"))
        {
            return RedirectToAction("Dashboard", "Organizer");
        }

        return RedirectToAction("Dashboard", "Staff");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        if (!IsLoggedIn)
        {
            return RedirectToAction(nameof(Login));
        }

        var result = await _authClientService.GetProfileAsync();
        if (!result.Success || result.Data is null)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction("Index", "Home");
        }

        return View(result.Data);
    }
}
