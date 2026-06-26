using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using UniversityActivities.Api.DTOs.Activities;
using UniversityActivities.Api.DTOs.Common;
using UniversityActivities.Api.Services.Interfaces;

namespace UniversityActivities.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManageActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ManageActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet("my")]
        [Authorize(Policy = "CanEditActivity")]
        public async Task<IActionResult> GetMyActivities()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                var response = new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Unauthorized"
                };

                return StatusCode(response.StatusCode, response);
            }

            var result = await _activityService.GetMyActiviesAsync(userId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Policy = "CanCreateActivity")]
        public async Task<IActionResult> CreateActivity(CreateActivityRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                var response = new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Unauthorized"
                };

                return StatusCode(response.StatusCode, response);
            }

            var result = await _activityService.CreateActivityAsync(request, userId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CanEditActivity")]
        public async Task<IActionResult> UpdateActivity(int id, UpdateActivityRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                var response = new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Unauthorized"
                };

                return StatusCode(response.StatusCode, response);
            }

            var isAdmin = User.IsInRole("Admin");

            var result = await _activityService.UpdateActivityAsync(
                id,
                request,
                userId,
                isAdmin);

            return StatusCode(result.StatusCode, result);
        }
    }
}
