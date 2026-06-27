using UniversityActivities.Api.Application.Common;
using UniversityActivities.Api.Application.DTOs.Registrations;

namespace UniversityActivities.Api.Application.Interfaces
{
    public interface IRegistrationService
    {
        Task<Result<RegistrationResponse>> RegisterActivityAsync(
            int activityId,
            string studentId);

        Task<Result<IEnumerable<RegistrationResponse>>> GetMyRegistrationsAsync(
            string studentId);

        Task<Result<RegistrationResponse>> CancelRegistrationAsync(
            int registrationId,
            string studentId);

        Task<Result<IEnumerable<RegistrationResponse>>> GetRegistrationsByActivityAsync(
            int activityId,
            string userId,
            bool isAdminOrStaff);

        Task<Result<RegistrationResponse>> UpdateParticipantStatusAsync(
            int registrationId,
            UpdateRegistrationStatusRequest request,
            string organizerId);
    }
}


