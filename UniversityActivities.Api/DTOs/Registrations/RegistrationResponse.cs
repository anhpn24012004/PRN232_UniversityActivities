using UniversityActivities.Api.Models;

namespace UniversityActivities.Api.DTOs.Registrations
{
    public class RegistrationResponse
    {
        public int Id { get; set; }

        public int ActivityId { get; set; }

        public string ActivityTitle { get; set; } = string.Empty;

        public string StudentId { get; set; } = string.Empty;

        public string StudentName { get; set;} = string.Empty;

        public DateTime RegisteredAt { get; set; }

        public RegistrationStatus Status { get; set; }
    }
}
