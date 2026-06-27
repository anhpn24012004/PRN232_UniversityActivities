using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System.Security.Claims;
using UniversityActivities.Api.DTOs.Common;
using UniversityActivities.Api.Services.Interfaces;

namespace UniversityActivities.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CanViewRegistrations")]
    public class ActivityRegistrationsController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public ActivityRegistrationsController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpGet("activity/{activityId}")]
        public async Task<IActionResult> GetRegistrationsByActivity(int activityId)
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

            var isAdminOrStaff =
                User.IsInRole("Admin") || User.IsInRole("Staff");

            var result =
                await _registrationService.GetRegistrationsByActivityAsync(
                    activityId,
                    userId,
                    isAdminOrStaff);

            return StatusCode(result.StatusCode, result);
        }
    }
}
