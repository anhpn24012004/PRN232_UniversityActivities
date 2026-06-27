using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityActivities.Api.Repositories.Interfaces;
using UniversityActivities.Api.Services.Interfaces;

namespace UniversityActivities.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "OnlyStaffOrAdmin")]
    public class StaffActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public StaffActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet("organized")]
        public async Task<IActionResult> GetOrganizedActivities()
        {
            var result = await _activityService.GetOrganizedActivitiesAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Pending")]
        public async Task<IActionResult> GetPendingActivities()
        {
            var result = await _activityService.GetPendingActivitiesAsync();

            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveActivity(int id)
        {
            var result = await _activityService.ApproveActivityAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectActivity(int id)
        {
            var result = await _activityService.RejectActivityAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
