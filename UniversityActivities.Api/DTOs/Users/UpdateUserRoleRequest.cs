using System.ComponentModel.DataAnnotations;

namespace UniversityActivities.Api.DTOs.Users
{
    public class UpdateUserRoleRequest
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
