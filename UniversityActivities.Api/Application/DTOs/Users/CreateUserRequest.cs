using System.ComponentModel.DataAnnotations;

namespace UniversityActivities.Api.Application.DTOs.Users
{
    public class CreateUserRequest
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string? StudentCode { get; set; }

        public string Department { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}


