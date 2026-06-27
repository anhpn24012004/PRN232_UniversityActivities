using WebApp_Client.Mvc.Models.Common;
using WebApp_Client.Mvc.Models.Registrations;

namespace WebApp_Client.Mvc;

public class RegistrationClientService
{
    private readonly ApiClient _apiClient;

    public RegistrationClientService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<IEnumerable<RegistrationResponse>>> GetByActivityAsync(int activityId)
    {
        return _apiClient.GetAsync<IEnumerable<RegistrationResponse>>($"/api/ActivityRegistrations/activity/{activityId}");
    }

    public Task<ApiResult<RegistrationResponse>> UpdateStatusAsync(int registrationId, int status)
    {
        var request = new UpdateRegistrationStatusRequest { Status = status };
        return _apiClient.PutAsync<UpdateRegistrationStatusRequest, RegistrationResponse>(
            $"/api/ActivityRegistrations/{registrationId}/status",
            request);
    }
}
