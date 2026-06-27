using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityActivities.Api.Application.Interfaces;

namespace UniversityActivities.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "OnlyAdmin")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _statisticsService.GetAdminStatisticsAsync();
            return StatusCode(result.StatusCode, result);
        }
    }
}

