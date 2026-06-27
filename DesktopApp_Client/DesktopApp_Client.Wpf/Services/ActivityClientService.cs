using DesktopApp_Client.Wpf.Models.Activities;
using DesktopApp_Client.Wpf.Models.Common;

namespace DesktopApp_Client.Wpf.Services;

public class ActivityClientService
{
    private readonly ApiClient _apiClient = new();

    public Task<ApiResult<IEnumerable<ActivityResponse>>> GetAllAsync()
    {
        return _apiClient.GetAsync<IEnumerable<ActivityResponse>>("/api/ManageActivities/all");
    }

    public Task<ApiResult<ActivityResponse>> CreateAsync(CreateActivityRequest request)
    {
        return _apiClient.PostAsync<CreateActivityRequest, ActivityResponse>("/api/ManageActivities", request);
    }

    public Task<ApiResult<ActivityResponse>> UpdateAsync(int id, UpdateActivityRequest request)
    {
        return _apiClient.PutAsync<UpdateActivityRequest, ActivityResponse>($"/api/ManageActivities/{id}", request);
    }

    public Task<ApiResult<ActivityResponse>> DeleteAsync(int id)
    {
        return _apiClient.DeleteAsync<ActivityResponse>($"/api/ManageActivities/{id}");
    }

    public Task<ApiResult<IEnumerable<ActivityResponse>>> GetPendingAsync()
    {
        return _apiClient.GetAsync<IEnumerable<ActivityResponse>>("/api/StaffActivities/pending");
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
