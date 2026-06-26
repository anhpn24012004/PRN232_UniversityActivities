using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityActivities.Api.Services.Interfaces;

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
        public async Task<IActionResult> GetApprovedActivities()
        {
            var result = await _activityService.GetApprovedActivitiesAsync();

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
