using Microsoft.AspNetCore.Mvc;
using WebApp_Client.Mvc.Models.ViewModels;

namespace WebApp_Client.Mvc.Controllers;

public class ActivityRegistrationsController : BaseClientController
{
    private readonly RegistrationClientService _registrationClientService;

    public ActivityRegistrationsController(RegistrationClientService registrationClientService)
    {
        _registrationClientService = registrationClientService;
    }

    public async Task<IActionResult> Index(int activityId, string? returnUrl = null)
    {
        if (!IsLoggedIn)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!HasRole("Organizer") && !HasRole("Staff"))
        {
            return RedirectToAction("AccessDenied", "Auth");
        }

        var result = await _registrationClientService.GetByActivityAsync(activityId);
        if (!result.Success)
        {
            return HandleApiFailure(result.StatusCode, result.Message);
        }

        return View(new ActivityRegistrationsViewModel
        {
            ActivityId = activityId,
            CanManageAttendance = HasRole("Organizer"),
            ReturnUrl = NormalizeReturnUrl(returnUrl),
            Registrations = result.Data ?? Enumerable.Empty<Models.Registrations.RegistrationResponse>()
        });
    }

    public async Task<IActionResult> Attendance(int activityId, string? returnUrl = null)
    {
        var guard = RequireRole("Organizer");
        if (guard is not EmptyResult) return guard;

        var result = await _registrationClientService.GetByActivityAsync(activityId);
        if (!result.Success)
        {
            return HandleApiFailure(result.StatusCode, result.Message);
        }

        return View(new ActivityRegistrationsViewModel
        {
            ActivityId = activityId,
            CanManageAttendance = true,
            ReturnUrl = NormalizeReturnUrl(returnUrl),
            Registrations = result.Data ?? Enumerable.Empty<Models.Registrations.RegistrationResponse>()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int activityId, int registrationId, int status, string? returnUrl = null)
    {
        var guard = RequireRole("Organizer");
        if (guard is not EmptyResult) return guard;

        var result = await _registrationClientService.UpdateStatusAsync(registrationId, status);
        TempData[result.Success ? "Success" : "Error"] = result.Message;

        return RedirectToAction(nameof(Attendance), new { activityId, returnUrl = NormalizeReturnUrl(returnUrl) });
    }

    private string NormalizeReturnUrl(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return returnUrl;
        }

        if (HasRole("Organizer"))
        {
            return Url.Action("MyActivities", "Organizer") ?? "/";
        }

        return Url.Action("PendingActivities", "Staff") ?? "/";
    }
}
