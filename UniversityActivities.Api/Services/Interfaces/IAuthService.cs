using UniversityActivities.Api.DTOs.Auth;
using UniversityActivities.Api.DTOs.Common;

namespace UniversityActivities.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<object>> RegisterAsync(RegisterRequest request);

        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);

        Task<ApiResponse<object>> GetProfileAsync(string userId);

        Task<ApiResponse<object>> UpdateProfileAsync(
            string userId,
            UpdateProfileRequest request);
    }
}