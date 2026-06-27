using UniversityActivities.Api.Application.Common;
using UniversityActivities.Api.Application.DTOs.Users;

namespace UniversityActivities.Api.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync();

        Task<Result<UserResponse>> GetUserByIdAsync(string id);

        Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request);

        Task<Result<UserResponse>> UpdateUserRoleAsync(
            string id,
            UpdateUserRoleRequest request);

        Task<Result<UserResponse>> LockUserAsync(string id);

        Task<Result<UserResponse>> UnlockUserAsync(string id);
    }
}

