using WebApp_Client.Mvc.Models.Activities;
using WebApp_Client.Mvc.Models.Common;

namespace WebApp_Client.Mvc;

public class OrganizerClientService
{
    private readonly ApiClient _apiClient;

    public OrganizerClientService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<IEnumerable<ActivityResponse>>> GetMyActivitiesAsync()
    {
        return _apiClient.GetAsync<IEnumerable<ActivityResponse>>("/api/ManageActivities/my");
    }

    public Task<ApiResult<ActivityResponse>> CreateAsync(CreateActivityRequest request)
    {
        return _apiClient.PostAsync<CreateActivityRequest, ActivityResponse>("/api/ManageActivities", request);
    }

    public Task<ApiResult<ActivityResponse>> UpdateAsync(int id, UpdateActivityRequest request)
    {
        return _apiClient.PutAsync<UpdateActivityRequest, ActivityResponse>($"/api/ManageActivities/{id}", request);
    }
}
