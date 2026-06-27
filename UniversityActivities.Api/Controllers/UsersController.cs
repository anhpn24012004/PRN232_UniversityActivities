using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityActivities.Api.DTOs.Users;
using UniversityActivities.Api.Services.Interfaces;

namespace UniversityActivities.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "OnlyAdmin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            var result = await _userService.CreateUserAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(
            string id,
            UpdateUserRoleRequest request)
        {
            var result = await _userService.UpdateUserRoleAsync(id, request);
            return StatusCode(result.StatusCode, result);
        }
    }
}