using DesktopApp_Client.Wpf.Models.Common;
using DesktopApp_Client.Wpf.Models.Users;

namespace DesktopApp_Client.Wpf.Services;

public class UserClientService
{
    private readonly ApiClient _apiClient = new();

    public Task<ApiResult<IEnumerable<UserResponse>>> GetUsersAsync()
    {
        return _apiClient.GetAsync<IEnumerable<UserResponse>>("/api/Users");
    }

    public Task<ApiResult<UserResponse>> GetUserAsync(string id)
    {
        return _apiClient.GetAsync<UserResponse>($"/api/Users/{id}");
    }

    public Task<ApiResult<UserResponse>> CreateUserAsync(CreateUserRequest request)
    {
        return _apiClient.PostAsync<CreateUserRequest, UserResponse>("/api/Users", request);
    }

    public Task<ApiResult<UserResponse>> UpdateRoleAsync(string id, UpdateUserRoleRequest request)
    {
        return _apiClient.PutAsync<UpdateUserRoleRequest, UserResponse>($"/api/Users/{id}/role", request);
    }

    public Task<ApiResult<UserResponse>> LockAsync(string id)
    {
        return _apiClient.PutAsync<object, UserResponse>($"/api/Users/{id}/lock", new { });
    }

    public Task<ApiResult<UserResponse>> UnlockAsync(string id)
    {
        return _apiClient.PutAsync<object, UserResponse>($"/api/Users/{id}/unlock", new { });
    }
}
