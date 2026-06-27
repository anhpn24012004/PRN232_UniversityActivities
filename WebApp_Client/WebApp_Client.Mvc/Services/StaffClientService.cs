using WebApp_Client.Mvc.Models.Activities;
using WebApp_Client.Mvc.Models.Common;

namespace WebApp_Client.Mvc;

public class StaffClientService
{
    private readonly ApiClient _apiClient;

    public StaffClientService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<IEnumerable<ActivityResponse>>> GetPendingActivitiesAsync()
    {
        return _apiClient.GetAsync<IEnumerable<ActivityResponse>>("/api/StaffActivities/pending");
    }

    public Task<ApiResult<IEnumerable<ActivityResponse>>> GetOrganizedActivitiesAsync()
    {
        return _apiClient.GetAsync<IEnumerable<ActivityResponse>>("/api/StaffActivities/organized");
    }

    public Task<ApiResult<ActivityResponse>> ApproveAsync(int id)
    {
        return _apiClient.PutAsync<object, ActivityResponse>($"/api/StaffActivities/{id}/approve", new { });
    }

    public Task<ApiResult<ActivityResponse>> RejectAsync(int id)
    {
        return _apiClient.PutAsync<object, ActivityResponse>($"/api/StaffActivities/{id}/reject", new { });
    }
}
