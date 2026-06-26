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
    }
}
