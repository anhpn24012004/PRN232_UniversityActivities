using WebApp_Client.Mvc.Models.Auth;
using WebApp_Client.Mvc.Models.Common;

namespace WebApp_Client.Mvc;

public class AuthClientService
{
    private readonly ApiClient _apiClient;

    public AuthClientService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        return _apiClient.PostAsync<LoginRequest, AuthResponse>("/api/Auth/login", request);
    }

    public Task<ApiResult<ProfileResponse>> GetProfileAsync()
    {
        return _apiClient.GetAsync<ProfileResponse>("/api/Auth/profile");
    }
}
