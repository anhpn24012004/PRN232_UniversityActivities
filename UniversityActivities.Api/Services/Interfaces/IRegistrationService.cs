using UniversityActivities.Api.DTOs.Common;
using UniversityActivities.Api.DTOs.Registrations;

namespace UniversityActivities.Api.Services.Interfaces
{
    public interface IRegistrationService
    {
        Task<ApiResponse<RegistrationResponse>> RegisterActivityAsync(
            int activityId,
            string studentId);

        Task<ApiResponse<IEnumerable<RegistrationResponse>>> GetMyRegistrationsAsync(
            string studentId);

        Task<ApiResponse<RegistrationResponse>> CancelRegistrationAsync(
            int registrationId,
            string studentId);

        Task<ApiResponse<IEnumerable<RegistrationResponse>>> GetRegistrationsByActivityAsync(
            int activityId,
            string userId,
            bool isAdminOrStaff);

        Task<ApiResponse<RegistrationResponse>> UpdateParticipantStatusAsync(
            int registrationId,
            UpdateRegistrationStatusRequest request,
            string organizerId);
    }
}
