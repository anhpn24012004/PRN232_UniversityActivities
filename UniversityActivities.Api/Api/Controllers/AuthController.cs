using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityActivities.Api.Application.DTOs.Auth;
using UniversityActivities.Api.Application.Common;
using UniversityActivities.Api.Application.Interfaces;

namespace UniversityActivities.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                var response = new Result<object>
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Unauthorized"
                };

                return StatusCode(response.StatusCode, response);
            }

            var result = await _authService.GetProfileAsync(userId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("profile")]
        [Authorize(Policy = "OnlyStudent")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                var response = new Result<object>
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Unauthorized"
                };

                return StatusCode(response.StatusCode, response);
            }

            var result = await _authService.UpdateProfileAsync(userId, request);
            return StatusCode(result.StatusCode, result);
        }
    }
}

