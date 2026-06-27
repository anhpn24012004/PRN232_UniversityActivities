using Microsoft.AspNetCore.Mvc;

namespace WebApp_Client.Mvc.Controllers;

public class HomeController : BaseClientController
{
    private readonly ApiClient _apiClient;

    public HomeController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Activities()
    {
        var result = await _apiClient.GetAsync<IEnumerable<Models.Activities.ActivityResponse>>("/api/Activities");
        if (!result.Success)
        {
            TempData["Error"] = result.Message;
        }

        return View(result.Data ?? Enumerable.Empty<Models.Activities.ActivityResponse>());
    }
}
