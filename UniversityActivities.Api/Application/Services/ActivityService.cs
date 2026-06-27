using UniversityActivities.Api.Application.DTOs.Activities;
using UniversityActivities.Api.Application.Common;
using UniversityActivities.Api.Domain.Entities;
using UniversityActivities.Api.Application.Interfaces;
using UniversityActivities.Api.Application.Mappings;

namespace UniversityActivities.Api.Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActivityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<ActivityResponse>>> GetApprovedActivitiesAsync(
            string? keyword = null,
            ActivityType? type = null,
            string? location = null)
        {
            var activities = await _unitOfWork.Activities.GetApprovedActivitiesAsync(
                keyword,
                type,
                location);

            var data = activities.Select(a => a.ToActivityResponse());

            return new Result<IEnumerable<ActivityResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get approved activities successfully",
                Data = data
            };
        }

        public async Task<Result<ActivityResponse>> GetApprovedActivityDetailAsync(int id)
        {
            var activity = await _unitOfWork.Activities.GetApprovedActivityByIdAsync(id);

            if (activity == null)
            {
                return new Result<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            return new Result<ActivityResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get activity detail successfully",
                Data = activity.ToActivityResponse()
            };
        }

        public async Task<Result<IEnumerable<ActivityResponse>>> GetMyActiviesAsync(string userId)
        {
            var activities = await _unitOfWork.Activities.GetActivitiesByOrganizerAsync(userId);

            var data = activities.Select(a => a.ToActivityResponse());

            return new Result<IEnumerable<ActivityResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get my activities successfully",
                Data = data
            };
        }

        public async Task<Result<ActivityResponse>> CreateActivityAsync(
            CreateActivityRequest request,
            string userId)
        {
            if (request.StartTime <= DateTime.Now)
            {
                return new Result<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Start time must be in the future"
                };
            }

            if (request.EndTime <= request.StartTime)
            {
                return new Result<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "End time must be after start time"
                };
            }

            var activity = new Activity
            {
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                MaxParticipants = request.MaxParticipants,
                Type = request.Type,
                Status = ActivityStatus.Pending,
                OrganizerId = userId
            };

            await _unitOfWork.Activities.AddAsync(activity);
            await _unitOfWork.SaveChangesAsync();

            return new Result<ActivityResponse>
            {
                Success = true,
                StatusCode = 201,
                Message = "Create activity successfully",
                Data = activity.ToActivityResponse()
            };
        }

        public async Task<Result<ActivityResponse>> UpdateActivityAsync(
            int id,
            UpdateActivityRequest request,
            string userId,
            bool isAdmin)
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(id);

            if (activity == null)
            {
                return new Result<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            if (!isAdmin && activity.OrganizerId != userId)
            {
                return new Result<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 403,
                    Message = "You can only edit your own activity"
                };
            }

            if (!isAdmin &&
                activity.Status != ActivityStatus.Pending &&
                activity.Status != ActivityStatus.Rejected)
            {
                return new Result<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Organizer can only edit pending or rejected activity"
                };
            }

            if (!isAdmin && activity.StartTime <= DateTime.Now)
            {
                return new Result<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Organizer cannot edit activity that has already started"
                };
            }

            if (request.StartTime <= DateTime.Now)
            {
                return new Result<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Start time must be in the future"
                };
            }

            if (request.EndTime <= request.StartTime)
            {
                return new Result<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "End time must be after start time"
                };
            }

            activity.Title = request.Title;
            activity.Description = request.Description;
            activity.Location = request.Location;
            activity.StartTime = request.StartTime;
            activity.EndTime = request.EndTime;
            activity.MaxParticipants = request.MaxParticipants;
            activity.Type = request.Type;

            _unitOfWork.Activities.Update(activity);
            await _unitOfWork.SaveChangesAsync();

            return new Result<ActivityResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Update activity successfully",
                Data = activity.ToActivityResponse()
            };
        }

        public async Task<Result<IEnumerable<ActivityResponse>>> GetPendingActivitiesAsync()
        {
            var activities = await _unitOfWork.Activities.GetPendingActivitiesAsync();

            var data = activities.Select(a => a.ToActivityResponse());

            return new Result<IEnumerable<ActivityResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get Pending activities successfully",
                Data = data
            };
        }

        public async Task<Result<ActivityResponse>> ApproveActivityAsync(int id)
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(id);

            if (activity == null)
            {
                return new Result<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            if (activity.Status != ActivityStatus.Pending)
            {
                return new Result<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Only Pending activity can be approved"
                };
            }

            activity.Status = ActivityStatus.Approved;

            _unitOfWork.Activities.Update(activity);
            await _unitOfWork.SaveChangesAsync();

            return new Result<ActivityResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Approve activity successfully",
                Data = activity.ToActivityResponse()
            };
        }

        public async Task<Result<ActivityResponse>> RejectActivityAsync(int id)
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(id);

            if (activity == null)
            {
                return new Result<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            if (activity.Status != ActivityStatus.Pending)
            {
                return new Result<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Only Pending activity can be Rejected"
                };
            }

            activity.Status = ActivityStatus.Rejected;
            await _unitOfWork.SaveChangesAsync();

            return new Result<ActivityResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Reject activity successfully",
                Data = activity.ToActivityResponse()
            };
        }

        public async Task<Result<object>> DeleteActivityAsync(int id)
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(id);

            if (activity == null)
            {
                return new Result<object>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            _unitOfWork.Activities.Delete(activity);
            await _unitOfWork.SaveChangesAsync();

            return new Result<object>
            {
                Success = true,
                StatusCode = 200,
                Message = "Delete activity successfully"
            };
        }

        public async Task<Result<IEnumerable<ActivityResponse>>> GetAllActivitiesAsync()
        {
            var activities = await _unitOfWork.Activities.GetAllActivitiesAsync();

            var data = activities.Select(a => a.ToActivityResponse());

            return new Result<IEnumerable<ActivityResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get all activities successfully",
                Data = data
            };
        }

        public async Task<Result<IEnumerable<ActivityResponse>>> GetOrganizedActivitiesAsync()
        {
            var activities = await _unitOfWork.Activities.GetOrganizedActivitiesAsync();

            var data = activities.Select(a => a.ToActivityResponse());

            return new Result<IEnumerable<ActivityResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get organized activities successfully",
                Data = data
            };
        }
    }
}


