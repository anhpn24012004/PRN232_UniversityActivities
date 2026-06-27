using Microsoft.AspNetCore.Identity;

namespace UniversityActivities.Api.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? StudentCode { get; set; }
        public string Department { get; set; } = string.Empty;
    }
}


