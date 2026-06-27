using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using UniversityActivities.Api.Application.DTOs.Auth;
using UniversityActivities.Api.Application.Common;
using UniversityActivities.Api.Domain.Entities;
using UniversityActivities.Api.Application.Interfaces;

namespace UniversityActivities.Api.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtService _jwtService;

        public AuthService(UserManager<AppUser> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<Result<object>> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return new Result<object>
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

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return new Result<object>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Register failed",
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            await _userManager.AddToRoleAsync(user, "Student");

            return new Result<object>
            {
                Success = true,
                StatusCode = 201,
                Message = "Register Successfully",
                Data = new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    Role = "Student"
                }
            };
        }


        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return new Result<AuthResponse>
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Invalid email or password"
                };
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return new Result<AuthResponse>
                {
                    Success = false,
                    StatusCode = 403,
                    Message = "Account is locked"
                };
            }

            var validPassword = await _userManager.CheckPasswordAsync(
                user,
                request.Password);

            if (!validPassword)
            {
                return new Result<AuthResponse>
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Invalid email or password"
                };
            }

            var token = await _jwtService.GenerateTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            return new Result<AuthResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Login successfully",
                Data = new AuthResponse
                {
                    Token = token,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    Roles = roles
                }
            };
        }

        public async Task<Result<object>> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new Result<object>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "User not found"
                };
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new Result<object>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get profile successfully",
                Data = new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    user.DateOfBirth,
                    user.StudentCode,
                    user.Department,
                    Roles = roles
                }
            };
        }

        public async Task<Result<object>> UpdateProfileAsync(
            string userId,
            UpdateProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new Result<object>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "User not found"
                };
            }

            user.FullName = request.FullName;
            user.DateOfBirth = request.DateOfBirth;
            user.StudentCode = request.StudentCode;
            user.Department = request.Department;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new Result<object>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Update profile failed",
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new Result<object>
            {
                Success = true,
                StatusCode = 200,
                Message = "Update profile successfully",
                Data = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.DateOfBirth,
                    user.StudentCode,
                    user.Department,
                    Roles = roles
                }
            };
        }
    }
}


