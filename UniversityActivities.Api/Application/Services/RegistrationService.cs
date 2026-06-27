using UniversityActivities.Api.Application.Common;
using UniversityActivities.Api.Application.DTOs.Registrations;
using UniversityActivities.Api.Domain.Entities;
using UniversityActivities.Api.Application.Interfaces;
using UniversityActivities.Api.Application.Mappings;

namespace UniversityActivities.Api.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegistrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<Result<RegistrationResponse>> CancelRegistrationAsync(int registrationId, string studentId)
        {
            var registration = await _unitOfWork.Registrations.GetByIdAsync(registrationId);

            if (registration == null)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Registration not found"
                };
            }

            if (registration.StudentId != studentId)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 403,
                    Message = "You can only cancel your own registration"
                };
            }

            if (registration.Status != RegistrationStatus.Registered)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Only registered registration can be cancelled"
                };
            }

            if (registration.Activity == null)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            if (registration.Activity.StartTime <= DateTime.Now)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Cannot cancel registration after activity has started"
                };
            }

            registration.Status = RegistrationStatus.Cancelled;

            _unitOfWork.Registrations.Update(registration);
            await _unitOfWork.SaveChangesAsync();

            return new Result<RegistrationResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Cancel registration successfully",
                Data = registration.ToRegistrationResponse()
            };
        }

        public async Task<Result<IEnumerable<RegistrationResponse>>> GetMyRegistrationsAsync(string studentId)
        {
            var registrations = 
                await _unitOfWork.Registrations.GetRegistrationsByStudentAsync(studentId);

            var data = registrations.Select(r => r.ToRegistrationResponse());

            return new Result<IEnumerable<RegistrationResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get my registrations successfully",
                Data = data
            };
        }

        public async Task<Result<IEnumerable<RegistrationResponse>>> GetRegistrationsByActivityAsync(int activityId, string userId, bool isAdminOrStaff)
        {

            var activity = await _unitOfWork.Activities.GetByIdAsync(activityId);

            if (activity == null)
            {
                return new Result<IEnumerable<RegistrationResponse>>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            if (!isAdminOrStaff && activity.OrganizerId != userId)
            {
                return new Result<IEnumerable<RegistrationResponse>>
                {
                    Success = false,
                    StatusCode = 403,
                    Message = "You can only view registrations of your own activity"
                };
            }

            var registrations =
                await _unitOfWork.Registrations.GetRegistrationsByActivityAsync(activityId);

            var data = registrations.Select(r => r.ToRegistrationResponse());

            return new Result<IEnumerable<RegistrationResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get activity registrations successfully",
                Data = data
            };
        }

        public async Task<Result<RegistrationResponse>> RegisterActivityAsync(
    int activityId,
    string studentId)
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(activityId);

            if (activity == null)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            if (activity.Status != ActivityStatus.Approved)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Only approved activity can be registered"
                };
            }

            if (activity.StartTime <= DateTime.Now)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Cannot register activity that has already started"
                };
            }

            var existingRegistration =
                await _unitOfWork.Registrations.GetByActivityAndStudentAsync(
                    activityId,
                    studentId);

            if (existingRegistration != null &&
                existingRegistration.Status == RegistrationStatus.Registered)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 409,
                    Message = "You have already registered this activity"
                };
            }

            var currentParticipants =
                await _unitOfWork.Registrations.CountRegisteredStudentsAsync(activityId);

            if (currentParticipants >= activity.MaxParticipants)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 409,
                    Message = "Activity is full"
                };
            }

            ActivityRegistration registration;

            if (existingRegistration != null)
            {
                existingRegistration.RegisteredAt = DateTime.Now;
                existingRegistration.Status = RegistrationStatus.Registered;
                existingRegistration.Activity = activity;

                _unitOfWork.Registrations.Update(existingRegistration);
                registration = existingRegistration;
            }
            else
            {
                registration = new ActivityRegistration
                {
                    ActivityId = activityId,
                    StudentId = studentId,
                    RegisteredAt = DateTime.Now,
                    Status = RegistrationStatus.Registered,
                    Activity = activity
                };

                await _unitOfWork.Registrations.AddAsync(registration);
            }

            await _unitOfWork.SaveChangesAsync();

            return new Result<RegistrationResponse>
            {
                Success = true,
                StatusCode = 201,
                Message = "Register activity successfully",
                Data = registration.ToRegistrationResponse()
            };
        }

        public async Task<Result<RegistrationResponse>> UpdateParticipantStatusAsync(int registrationId, UpdateRegistrationStatusRequest request, string organizerId)
        {
            var registration = await _unitOfWork.Registrations.GetByIdAsync(registrationId);

            if (registration == null)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Registration not found"
                };
            }

            if (registration.Activity == null)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            if (registration.Activity.OrganizerId != organizerId)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 403,
                    Message = "You can only update attendance for your own acitivity"
                };
            }

            if (registration.Activity.EndTime > DateTime.Now)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Can only update attendance after activity has ended"
                };
            }

            if (registration.Status != RegistrationStatus.Registered)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Only registered participants can be marked as attended or absent"
                };
            }

            if (request.Status != RegistrationStatus.Attended &&
                request.Status != RegistrationStatus.Absent)
            {
                return new Result<RegistrationResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Status must be Attended or Absent"
                };
            }

            registration.Status = request.Status;

            _unitOfWork.Registrations.Update(registration);
            await _unitOfWork.SaveChangesAsync();

            return new Result<RegistrationResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Update attendance successfully",
                Data = registration.ToRegistrationResponse()
            };
        }
    }
}


