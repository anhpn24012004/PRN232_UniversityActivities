using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityActivities.Api.Application.Common;

namespace UniversityActivities.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetHome()
        {
            var response = new Result<object>
            {
                Success = true,
                StatusCode = 200,
                Message = "Welcome to University Activities API",
                Data = new
                {
                    Project = "University Activities Management System",
                    Description = "API for managing university extracurricular activities, registrations, approvals, attendance, users and statistics.",
                    MainFeatures = new[]
                    {
                        "View approved activities",
                        "Register and login",
                        "Student activity registration",
                        "Organizer activity management",
                        "Staff approval",
                        "Admin user management",
                        "Admin statistics"
                    }
                }
            };

            return StatusCode(response.StatusCode, response);
        }
    }
}

