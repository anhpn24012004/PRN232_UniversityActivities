using UniversityActivities.Api.Models;

namespace UniversityActivities.Api.DTOs.Registrations
{
    public class UpdateRegistrationStatusRequest
    {
        public RegistrationStatus Status { get; set; }
    }
}
