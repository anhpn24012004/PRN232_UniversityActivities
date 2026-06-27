using UniversityActivities.Api.Application.DTOs.Auth;
using UniversityActivities.Api.Application.Common;

namespace UniversityActivities.Api.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<object>> RegisterAsync(RegisterRequest request);

        Task<Result<AuthResponse>> LoginAsync(LoginRequest request);

        Task<Result<object>> GetProfileAsync(string userId);

        Task<Result<object>> UpdateProfileAsync(
            string userId,
            UpdateProfileRequest request);
    }
}

