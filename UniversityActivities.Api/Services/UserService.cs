using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityActivities.Api.DTOs.Common;
using UniversityActivities.Api.DTOs.Users;
using UniversityActivities.Api.Models;
using UniversityActivities.Api.Services.Interfaces;

namespace UniversityActivities.Api.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApiResponse<IEnumerable<UserResponse>>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            var data = new List<UserResponse>();

            foreach (var user in users)
            {
                data.Add(await MapToUserResponseAsync(user));
            }

            return new ApiResponse<IEnumerable<UserResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get users successfully",
                Data = data
            };
        }

        public async Task<ApiResponse<UserResponse>> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new ApiResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "User not found"
                };
            }

            return new ApiResponse<UserResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get user detail successfully",
                Data = await MapToUserResponseAsync(user)
            };
        }

        public async Task<ApiResponse<UserResponse>> CreateUserAsync(CreateUserRequest request)
        {
            var roleExists = await _roleManager.RoleExistsAsync(request.Role);

            if (!roleExists)
            {
                return new ApiResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Role does not exist"
                };
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return new ApiResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Email already exists"
                };
            }

            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                DateOfBirth = request.DateOfBirth,
                StudentCode = request.StudentCode,
                Department = request.Department,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
            {
                return new ApiResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Create user failed",
                    Errors = createResult.Errors.Select(e => e.Description)
                };
            }

            var roleResult = await _userManager.AddToRoleAsync(user, request.Role);

            if (!roleResult.Succeeded)
            {
                return new ApiResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Assign role failed",
                    Errors = roleResult.Errors.Select(e => e.Description)
                };
            }

            return new ApiResponse<UserResponse>
            {
                Success = true,
                StatusCode = 201,
                Message = "Create user successfully",
                Data = await MapToUserResponseAsync(user)
            };
        }

        public async Task<ApiResponse<UserResponse>> UpdateUserRoleAsync(
            string id,
            UpdateUserRoleRequest request)
        {
            var roleExists = await _roleManager.RoleExistsAsync(request.Role);

            if (!roleExists)
            {
                return new ApiResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Role does not exist"
                };
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new ApiResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "User not found"
                };
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!removeResult.Succeeded)
                {
                    return new ApiResponse<UserResponse>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Remove old roles failed",
                        Errors = removeResult.Errors.Select(e => e.Description)
                    };
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, request.Role);

            if (!addResult.Succeeded)
            {
                return new ApiResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Update role failed",
                    Errors = addResult.Errors.Select(e => e.Description)
                };
            }

            return new ApiResponse<UserResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Update user role successfully",
                Data = await MapToUserResponseAsync(user)
            };
        }

        private async Task<UserResponse> MapToUserResponseAsync(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                StudentCode = user.StudentCode,
                Department = user.Department,
                Roles = roles
            };
        }
    }
}
