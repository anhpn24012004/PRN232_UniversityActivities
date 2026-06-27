using System.ComponentModel.DataAnnotations;

namespace UniversityActivities.Api.Application.DTOs.Users
{
    public class UpdateUserRoleRequest
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}


