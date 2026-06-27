using System.ComponentModel.DataAnnotations;

namespace UniversityActivities.Api.Application.DTOs.Auth
{
    public class UpdateProfileRequest
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string? StudentCode { get; set; }

        [Required]
        public string Department { get; set; } = string.Empty;
    }
}

