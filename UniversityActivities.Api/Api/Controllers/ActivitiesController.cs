using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityActivities.Api.Domain.Entities;
using UniversityActivities.Api.Application.Interfaces;

namespace UniversityActivities.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetApprovedActivities(
            [FromQuery] string? keyword,
            [FromQuery] ActivityType? type,
            [FromQuery] string? location)
        {
            var result = await _activityService.GetApprovedActivitiesAsync(
                keyword,
                type,
                location);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetApprovedActivityDetail(int id)
        {
            var result = await _activityService.GetApprovedActivityDetailAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}


