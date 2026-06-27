using Microsoft.AspNetCore.Mvc;
using WebApp_Client.Mvc.Models.Activities;
using WebApp_Client.Mvc.Models.ViewModels;

namespace WebApp_Client.Mvc.Controllers;

public class StaffController : BaseClientController
{
    private readonly StaffClientService _staffClientService;

    public StaffController(StaffClientService staffClientService)
    {
        _staffClientService = staffClientService;
    }

    public IActionResult Dashboard()
    {
        var guard = RequireRole("Staff");
        if (guard is not EmptyResult) return guard;
        return View();
    }

    public async Task<IActionResult> PendingActivities()
    {
        var guard = RequireRole("Staff");
        if (guard is not EmptyResult) return guard;

        var result = await _staffClientService.GetPendingActivitiesAsync();
        if (!result.Success)
        {
            return HandleApiFailure(result.StatusCode, result.Message);
        }

        return View(result.Data ?? Enumerable.Empty<ActivityResponse>());
    }

    public async Task<IActionResult> OrganizedActivities()
    {
        var guard = RequireRole("Staff");
        if (guard is not EmptyResult) return guard;

        var result = await _staffClientService.GetOrganizedActivitiesAsync();
        if (!result.Success)
        {
            return HandleApiFailure(result.StatusCode, result.Message);
        }

        return View("OrganizedActivities", result.Data ?? Enumerable.Empty<ActivityResponse>());
    }

    public Task<IActionResult> ApprovedActivities()
    {
        return OrganizedActivities();
    }

    public async Task<IActionResult> Registrations()
    {
        var guard = RequireRole("Staff");
        if (guard is not EmptyResult) return guard;

        var pendingResult = await _staffClientService.GetPendingActivitiesAsync();
        if (!pendingResult.Success)
        {
            return HandleApiFailure(pendingResult.StatusCode, pendingResult.Message);
        }

        var approvedResult = await _staffClientService.GetOrganizedActivitiesAsync();
        if (!approvedResult.Success)
        {
            return HandleApiFailure(approvedResult.StatusCode, approvedResult.Message);
        }

        return View(new StaffRegistrationsViewModel
        {
            PendingActivities = pendingResult.Data ?? Enumerable.Empty<ActivityResponse>(),
            ApprovedActivities = approvedResult.Data ?? Enumerable.Empty<ActivityResponse>()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var guard = RequireRole("Staff");
        if (guard is not EmptyResult) return guard;

        var result = await _staffClientService.ApproveAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(PendingActivities));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var guard = RequireRole("Staff");
        if (guard is not EmptyResult) return guard;

        var result = await _staffClientService.RejectAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(PendingActivities));
    }
}
