using UniversityActivities.Api.DTOs.Common;
using UniversityActivities.Api.DTOs.Registrations;
using UniversityActivities.Api.Models;
using UniversityActivities.Api.Repositories.Interfaces;
using UniversityActivities.Api.Services.Interfaces;

namespace UniversityActivities.Api.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IRegistrationRepository _registrationRepository;

        public RegistrationService(IActivityRepository activityRepository, IRegistrationRepository registrationRepository)
        {
            _activityRepository = activityRepository;
            _registrationRepository = registrationRepository;
        }


        public async Task<ApiResponse<RegistrationResponse>> CancelRegistrationAsync(int registrationId, string studentId)
        {
            var registration = await _registrationRepository.GetByIdAsync(registrationId);

            if (registration == null)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Registration not found"
                };
            }

            if (registration.StudentId != studentId)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 403,
                    Message = "You can only cancel your own registration"
                };
            }

            if (registration.Status != RegistrationStatus.Registered)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Only registered registration can be cancelled"
                };
            }

            if (registration.Activity == null)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            if (registration.Activity.StartTime <= DateTime.Now)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Cannot cancel registration after activity has started"
                };
            }

            registration.Status = RegistrationStatus.Cancelled;

            _registrationRepository.Update(registration);
            await _registrationRepository.SaveChangeAsync();

            return new ApiResponse<RegistrationResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Cancel registration successfully",
                Data = MapToRegistrationResponse(registration)
            };
        }

        public async Task<ApiResponse<IEnumerable<RegistrationResponse>>> GetMyRegistrationsAsync(string studentId)
        {
            var registrations = 
                await _registrationRepository.GetRegistrationsByStudentAsync(studentId);

            var data = registrations.Select(MapToRegistrationResponse);

            return new ApiResponse<IEnumerable<RegistrationResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get my registrations successfully",
                Data = data
            };
        }

        public async Task<ApiResponse<IEnumerable<RegistrationResponse>>> GetRegistrationsByActivityAsync(int activityId, string userId, bool isAdminOrStaff)
        {

            var activity = await _activityRepository.GetByIdAsync(activityId);

            if (activity == null)
            {
                return new ApiResponse<IEnumerable<RegistrationResponse>>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            if (!isAdminOrStaff && activity.OrganizerId != userId)
            {
                return new ApiResponse<IEnumerable<RegistrationResponse>>
                {
                    Success = false,
                    StatusCode = 403,
                    Message = "You can only view registrations of your own activity"
                };
            }

            var registrations =
                await _registrationRepository.GetRegistrationsByActivityAsync(activityId);

            var data = registrations.Select(MapToRegistrationResponse);

            return new ApiResponse<IEnumerable<RegistrationResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get activity registrations successfully",
                Data = data
            };
        }

        public async Task<ApiResponse<RegistrationResponse>> RegisterActivityAsync(
    int activityId,
    string studentId)
        {
            var activity = await _activityRepository.GetByIdAsync(activityId);

            if (activity == null)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            if (activity.Status != ActivityStatus.Approved)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Only approved activity can be registered"
                };
            }

            if (activity.StartTime <= DateTime.Now)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Cannot register activity that has already started"
                };
            }

            var existingRegistration =
                await _registrationRepository.GetByActivityAndStudentAsync(
                    activityId,
                    studentId);

            if (existingRegistration != null &&
                existingRegistration.Status == RegistrationStatus.Registered)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 409,
                    Message = "You have already registered this activity"
                };
            }

            var currentParticipants =
                await _registrationRepository.CountRegisteredStudentsAsync(activityId);

            if (currentParticipants >= activity.MaxParticipants)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 409,
                    Message = "Activity is full"
                };
            }

            var registration = new ActivityRegistration
            {
                ActivityId = activityId,
                StudentId = studentId,
                RegisteredAt = DateTime.Now,
                Status = RegistrationStatus.Registered
            };

            await _registrationRepository.AddAsync(registration);
            await _registrationRepository.SaveChangeAsync();

            registration.Activity = activity;

            return new ApiResponse<RegistrationResponse>
            {
                Success = true,
                StatusCode = 201,
                Message = "Register activity successfully",
                Data = MapToRegistrationResponse(registration)
            };
        }

        private RegistrationResponse MapToRegistrationResponse(ActivityRegistration registration)
        {
            return new RegistrationResponse
            {
                Id = registration.Id,
                ActivityId = registration.ActivityId,
                ActivityTitle = registration.Activity?.Title ?? string.Empty,
                StudentId = registration.StudentId,
                StudentName = registration.Student?.FullName ?? string.Empty,
                RegisteredAt = registration.RegisteredAt,
                Status = registration.Status,
            };
        }
    }
}
