using UniversityActivities.Api.Application.DTOs.Registrations;
using UniversityActivities.Api.Domain.Entities;

namespace UniversityActivities.Api.Application.Mappings
{
    public static class RegistrationMappingExtensions
    {
        public static RegistrationResponse ToRegistrationResponse(this ActivityRegistration registration)
        {
            return new RegistrationResponse
            {
                Id = registration.Id,
                ActivityId = registration.ActivityId,
                ActivityTitle = registration.Activity?.Title ?? string.Empty,
                StudentId = registration.StudentId,
                StudentName = registration.Student?.FullName ?? string.Empty,
                RegisteredAt = registration.RegisteredAt,
                Status = registration.Status
            };
        }
    }
}
