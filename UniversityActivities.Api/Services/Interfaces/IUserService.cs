using UniversityActivities.Api.DTOs.Common;
using UniversityActivities.Api.DTOs.Users;

namespace UniversityActivities.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<IEnumerable<UserResponse>>> GetAllUsersAsync();

        Task<ApiResponse<UserResponse>> GetUserByIdAsync(string id);

        Task<ApiResponse<UserResponse>> CreateUserAsync(CreateUserRequest request);

        Task<ApiResponse<UserResponse>> UpdateUserRoleAsync(
            string id,
            UpdateUserRoleRequest request);
    }
}