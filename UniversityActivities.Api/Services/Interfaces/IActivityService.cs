using UniversityActivities.Api.DTOs.Activities;
using UniversityActivities.Api.DTOs.Common;

namespace UniversityActivities.Api.Services.Interfaces
{
    public interface IActivityService
    {
        Task<ApiResponse<IEnumerable<ActivityResponse>>> GetApprovedActivitiesAsync();

        Task<ApiResponse<ActivityResponse>> GetApprovedActivityDetailAsync(int id);
    }
}
