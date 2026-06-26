using UniversityActivities.Api.DTOs.Activities;
using UniversityActivities.Api.DTOs.Common;

namespace UniversityActivities.Api.Services.Interfaces
{
    public interface IActivityService
    {
        Task<ApiResponse<IEnumerable<ActivityResponse>>> GetApprovedActivitiesAsync();

        Task<ApiResponse<ActivityResponse>> GetApprovedActivityDetailAsync(int id);

        Task<ApiResponse<IEnumerable<ActivityResponse>>> GetMyActiviesAsync(string userId);

        Task<ApiResponse<ActivityResponse>> CreateActivityAsync(
            CreateActivityRequest request,
            string useId);

        Task<ApiResponse<ActivityResponse>> UpdateActivityAsync(
            int id,
            UpdateActivityRequest request,
            string useId,
            bool isAdmin);
    }
}
