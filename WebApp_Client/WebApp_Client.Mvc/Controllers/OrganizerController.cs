using Microsoft.AspNetCore.Mvc;
using WebApp_Client.Mvc.Models.Activities;

namespace WebApp_Client.Mvc.Controllers;

public class OrganizerController : BaseClientController
{
    private readonly OrganizerClientService _organizerClientService;

    public OrganizerController(OrganizerClientService organizerClientService)
    {
        _organizerClientService = organizerClientService;
    }

    public IActionResult Dashboard()
    {
        var guard = RequireRole("Organizer");
        if (guard is not EmptyResult) return guard;
        return View();
    }

    public async Task<IActionResult> MyActivities()
    {
        var guard = RequireRole("Organizer");
        if (guard is not EmptyResult) return guard;

        var result = await _organizerClientService.GetMyActivitiesAsync();
        if (!result.Success)
        {
            return HandleApiFailure(result.StatusCode, result.Message);
        }

        return View(result.Data ?? Enumerable.Empty<ActivityResponse>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        var guard = RequireRole("Organizer");
        if (guard is not EmptyResult) return guard;
        return View(new CreateActivityRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateActivityRequest request)
    {
        var guard = RequireRole("Organizer");
        if (guard is not EmptyResult) return guard;
        ValidateActivityTime(request.StartTime, request.EndTime);

        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _organizerClientService.CreateAsync(request);
        if (!result.Success)
        {
            ViewBag.Error = result.Message;
            return View(request);
        }

        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(MyActivities));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var guard = RequireRole("Organizer");
        if (guard is not EmptyResult) return guard;

        var result = await _organizerClientService.GetMyActivitiesAsync();
        if (!result.Success)
        {
            return HandleApiFailure(result.StatusCode, result.Message);
        }

        var activity = result.Data?.FirstOrDefault(a => a.Id == id);
        if (activity is null)
        {
            TempData["Error"] = "Activity not found.";
            return RedirectToAction(nameof(MyActivities));
        }

        return View(new UpdateActivityRequest
        {
            Id = activity.Id,
            Title = activity.Title,
            Description = activity.Description,
            Location = activity.Location,
            StartTime = activity.StartTime,
            EndTime = activity.EndTime,
            MaxParticipants = activity.MaxParticipants,
            Type = activity.Type
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateActivityRequest request)
    {
        var guard = RequireRole("Organizer");
        if (guard is not EmptyResult) return guard;
        ValidateActivityTime(request.StartTime, request.EndTime);

        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _organizerClientService.UpdateAsync(id, request);
        if (!result.Success)
        {
            ViewBag.Error = result.Message;
            return View(request);
        }

        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(MyActivities));
    }

    private void ValidateActivityTime(DateTime? startTime, DateTime? endTime)
    {
        if (startTime.HasValue && endTime.HasValue && endTime <= startTime)
        {
            ModelState.AddModelError(nameof(CreateActivityRequest.EndTime), "EndTime must be greater than StartTime.");
        }
    }
}
