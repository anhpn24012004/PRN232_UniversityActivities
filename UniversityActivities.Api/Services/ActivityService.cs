using UniversityActivities.Api.DTOs.Activities;
using UniversityActivities.Api.DTOs.Common;
using UniversityActivities.Api.Models;
using UniversityActivities.Api.Repositories.Interfaces;
using UniversityActivities.Api.Services.Interfaces;

namespace UniversityActivities.Api.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;

        public ActivityService(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }

        public async Task<ApiResponse<IEnumerable<ActivityResponse>>> GetApprovedActivitiesAsync()
        {
            var activities = await _activityRepository.GetApprovedActivitiesAsync();

            var data = activities.Select(MapToActivityResponse);

            return new ApiResponse<IEnumerable<ActivityResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get approved activities successfully",
                Data = data
            };
        }

        public async Task<ApiResponse<ActivityResponse>> GetApprovedActivityDetailAsync(int id)
        {
            var activity = await _activityRepository.GetApprovedActivityByIdAsync(id);

            if (activity == null)
            {
                return new ApiResponse<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            return new ApiResponse<ActivityResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get activity detail successfully",
                Data = MapToActivityResponse(activity)
            };
        }

        public async Task<ApiResponse<IEnumerable<ActivityResponse>>> GetMyActiviesAsync(string userId)
        {
            var activities = await _activityRepository.GetActivitiesByOrganizerAsync(userId);

            var data = activities.Select(MapToActivityResponse);

            return new ApiResponse<IEnumerable<ActivityResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get my activities successfully",
                Data = data
            };
        }

        public async Task<ApiResponse<ActivityResponse>> CreateActivityAsync(
            CreateActivityRequest request,
            string userId)
        {
            if (request.StartTime <= DateTime.Now)
            {
                return new ApiResponse<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Start time must be in the future"
                };
            }

            if (request.EndTime <= request.StartTime)
            {
                return new ApiResponse<ActivityResponse>
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

            await _activityRepository.AddAsync(activity);
            await _activityRepository.SaveChangesAsync();

            return new ApiResponse<ActivityResponse>
            {
                Success = true,
                StatusCode = 201,
                Message = "Create activity successfully",
                Data = MapToActivityResponse(activity)
            };
        }

        public async Task<ApiResponse<ActivityResponse>> UpdateActivityAsync(
            int id,
            UpdateActivityRequest request,
            string userId,
            bool isAdmin)
        {
            var activity = await _activityRepository.GetByIdAsync(id);

            if (activity == null)
            {
                return new ApiResponse<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            if (!isAdmin && activity.OrganizerId != userId)
            {
                return new ApiResponse<ActivityResponse>
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
                return new ApiResponse<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Organizer can only edit pending or rejected activity"
                };
            }

            if (!isAdmin && activity.StartTime <= DateTime.Now)
            {
                return new ApiResponse<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Organizer cannot edit activity that has already started"
                };
            }

            if (request.StartTime <= DateTime.Now)
            {
                return new ApiResponse<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Start time must be in the future"
                };
            }

            if (request.EndTime <= request.StartTime)
            {
                return new ApiResponse<ActivityResponse>
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

            _activityRepository.Update(activity);
            await _activityRepository.SaveChangesAsync();

            return new ApiResponse<ActivityResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Update activity successfully",
                Data = MapToActivityResponse(activity)
            };
        }

        private static ActivityResponse MapToActivityResponse(Activity activity)
        {
            return new ActivityResponse
            {
                Id = activity.Id,
                Title = activity.Title,
                Description = activity.Description,
                Location = activity.Location,
                StartTime = activity.StartTime,
                EndTime = activity.EndTime,
                MaxParticipants = activity.MaxParticipants,
                Type = activity.Type,
                Status = activity.Status,
                OrganizerName = activity.Organizer?.FullName ?? string.Empty
            };
        }

        public async Task<ApiResponse<IEnumerable<ActivityResponse>>> GetPendingActivitiesAsync()
        {
            var activities = await _activityRepository.GetPendingActivitiesAsync();

            var data = activities.Select(MapToActivityResponse);

            return new ApiResponse<IEnumerable<ActivityResponse>>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get Pending activities successfully",
                Data = data
            };
        }

        public async Task<ApiResponse<ActivityResponse>> ApproveActivityAsync(int id)
        {
            var activity = await _activityRepository.GetByIdAsync(id);

            if (activity == null)
            {
                return new ApiResponse<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            if (activity.Status != ActivityStatus.Pending)
            {
                return new ApiResponse<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Only Pending activity can be approved"
                };
            }

            activity.Status = ActivityStatus.Approved;

            _activityRepository.Update(activity);
            await _activityRepository.SaveChangesAsync();

            return new ApiResponse<ActivityResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Approve activity successfully",
                Data = MapToActivityResponse(activity)
            };
        }

        public async Task<ApiResponse<ActivityResponse>> RejectActivityAsync(int id)
        {
            var activity = await _activityRepository.GetByIdAsync(id);

            if (activity == null)
            {
                return new ApiResponse<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Activity not found"
                };
            }

            if (activity.Status != ActivityStatus.Pending)
            {
                return new ApiResponse<ActivityResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Only Pending activity can be Rejected"
                };
            }

            activity.Status = ActivityStatus.Rejected;
            await _activityRepository.SaveChangesAsync();

            return new ApiResponse<ActivityResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Reject activity successfully",
                Data = MapToActivityResponse(activity)
            };
        }
    }
}
