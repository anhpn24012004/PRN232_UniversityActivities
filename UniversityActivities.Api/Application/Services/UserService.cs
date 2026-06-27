using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityActivities.Api.Application.Common;
using UniversityActivities.Api.Application.DTOs.Users;
using UniversityActivities.Api.Domain.Entities;
using UniversityActivities.Api.Application.Interfaces;
using UniversityActivities.Api.Application.Mappings;

namespace UniversityActivities.Api.Application.Services
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

        public async Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            var data = new List<UserResponse>();

            foreach (var user in users)
            {
                data.Add(await MapToUserResponseAsync(user));
            }

            return new Result<IEnumerable<UserResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get users successfully",
                Data = data
            };
        }

        public async Task<Result<UserResponse>> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new Result<UserResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "User not found"
                };
            }

            return new Result<UserResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get user detail successfully",
                Data = await MapToUserResponseAsync(user)
            };
        }

        public async Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request)
        {
            var roleExists = await _roleManager.RoleExistsAsync(request.Role);

            if (!roleExists)
            {
                return new Result<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Role does not exist"
                };
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return new Result<UserResponse>
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
                return new Result<UserResponse>
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
                return new Result<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Assign role failed",
                    Errors = roleResult.Errors.Select(e => e.Description)
                };
            }

            return new Result<UserResponse>
            {
                Success = true,
                StatusCode = 201,
                Message = "Create user successfully",
                Data = await MapToUserResponseAsync(user)
            };
        }

        public async Task<Result<UserResponse>> UpdateUserRoleAsync(
            string id,
            UpdateUserRoleRequest request)
        {
            var roleExists = await _roleManager.RoleExistsAsync(request.Role);

            if (!roleExists)
            {
                return new Result<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Role does not exist"
                };
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new Result<UserResponse>
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
                    return new Result<UserResponse>
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
                return new Result<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Update role failed",
                    Errors = addResult.Errors.Select(e => e.Description)
                };
            }

            return new Result<UserResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Update user role successfully",
                Data = await MapToUserResponseAsync(user)
            };
        }

        public async Task<Result<UserResponse>> LockUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new Result<UserResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "User not found"
                };
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return new Result<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Admin account cannot be locked"
                };
            }

            await _userManager.SetLockoutEnabledAsync(user, true);
            var result = await _userManager.SetLockoutEndDateAsync(
                user,
                DateTimeOffset.UtcNow.AddYears(100));

            if (!result.Succeeded)
            {
                return new Result<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Lock user failed",
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return new Result<UserResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Lock user successfully",
                Data = await MapToUserResponseAsync(user)
            };
        }

        public async Task<Result<UserResponse>> UnlockUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new Result<UserResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "User not found"
                };
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (!result.Succeeded)
            {
                return new Result<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Unlock user failed",
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return new Result<UserResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Unlock user successfully",
                Data = await MapToUserResponseAsync(user)
            };
        }

        private async Task<UserResponse> MapToUserResponseAsync(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return user.ToUserResponse(roles);
        }
    }
}


