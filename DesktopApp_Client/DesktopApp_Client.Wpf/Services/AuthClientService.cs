using DesktopApp_Client.Wpf.Models.Auth;
using DesktopApp_Client.Wpf.Models.Common;

namespace DesktopApp_Client.Wpf.Services;

public class AuthClientService
{
    private readonly ApiClient _apiClient = new();

    public Task<ApiResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        return _apiClient.PostAsync<LoginRequest, AuthResponse>("/api/Auth/login", request);
    }
}
