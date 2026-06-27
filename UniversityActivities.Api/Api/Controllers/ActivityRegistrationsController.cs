using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System.Security.Claims;
using UniversityActivities.Api.Application.Common;
using UniversityActivities.Api.Application.DTOs.Registrations;
using UniversityActivities.Api.Application.Interfaces;

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
                var response = new Result<object>
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

        [HttpPut("{registrationId}/status")]
        [Authorize(Policy = "OnlyOrganizer")]
        public async Task<IActionResult> UpdateParticipantStatus(
            int registrationId,
            UpdateRegistrationStatusRequest request)
        {
            var organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (organizerId == null)
            {
                var response = new Result<object>
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Unauthorized"
                };

                return StatusCode(response.StatusCode, response);
            }

            var result =
                await _registrationService.UpdateParticipantStatusAsync(
                    registrationId,
                    request,
                    organizerId);

            return StatusCode(result.StatusCode, result);
        }
    }
}


