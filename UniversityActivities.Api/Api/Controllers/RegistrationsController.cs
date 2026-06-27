using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversityActivities.Api.Application.Common;
using UniversityActivities.Api.Application.Interfaces;

namespace UniversityActivities.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "OnlyStudent")]
    public class RegistrationsController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationsController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost("activities/{activityId}")]
        public async Task<IActionResult> RegisterActivity(int activityId)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (studentId == null)
            {
                var response = new Result<object>
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Unauthorized"
                };

                return StatusCode(response.StatusCode, response);
            }

            var result = await _registrationService.RegisterActivityAsync(
                activityId,
                studentId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyRegistrations()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (studentId == null)
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
                await _registrationService.GetMyRegistrationsAsync(studentId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelRegistration(int id)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (studentId == null)
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
                await _registrationService.CancelRegistrationAsync(id, studentId);

            return StatusCode(result.StatusCode, result);
        }
    }
}


