using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using UniversityActivities.Api.Api.Authorization;
using UniversityActivities.Api.Application.DTOs.Activities;
using UniversityActivities.Api.Application.Common;
using UniversityActivities.Api.Application.Interfaces;

namespace UniversityActivities.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManageActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUnitOfWork _unitOfWork;

        public ManageActivitiesController(
            IActivityService activityService,
            IAuthorizationService authorizationService,
            IUnitOfWork unitOfWork)
        {
            _activityService = activityService;
            _authorizationService = authorizationService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("all")]
        [Authorize(Policy = "OnlyAdmin")]
        public async Task<IActionResult> GetAllActivities()
        {
            var result = await _activityService.GetAllActivitiesAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("my")]
        [Authorize(Policy = "CanEditActivity")]
        public async Task<IActionResult> GetMyActivities()
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
                var response = new Result<object>
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
                var response = new Result<object>
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Unauthorized"
                };

                return StatusCode(response.StatusCode, response);
            }

            var isAdmin = User.IsInRole("Admin");

            var activity = await _unitOfWork.Activities.GetByIdAsync(id);

            if (activity == null)
            {
                var response = new Result<object>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };

                return StatusCode(response.StatusCode, response);
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User,
                activity,
                new CanEditOwnActivityRequirement());

            if (!authorizationResult.Succeeded)
            {
                var response = new Result<object>
                {
                    Success = false,
                    StatusCode = 403,
                    Message = "You can only edit your own activity"
                };

                return StatusCode(response.StatusCode, response);
            }

            var result = await _activityService.UpdateActivityAsync(
                id,
                request,
                userId,
                isAdmin);

            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CanDeleteActivity")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var result = await _activityService.DeleteActivityAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}


