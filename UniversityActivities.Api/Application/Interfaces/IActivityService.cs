using UniversityActivities.Api.Application.DTOs.Activities;
using UniversityActivities.Api.Application.Common;
using UniversityActivities.Api.Domain.Entities;

namespace UniversityActivities.Api.Application.Interfaces
{
    public interface IActivityService
    {
        Task<Result<IEnumerable<ActivityResponse>>> GetApprovedActivitiesAsync(
            string? keyword = null,
            ActivityType? type = null,
            string? location = null);

        Task<Result<IEnumerable<ActivityResponse>>> GetAllActivitiesAsync();

        Task<Result<IEnumerable<ActivityResponse>>> GetOrganizedActivitiesAsync();

        Task<Result<ActivityResponse>> GetApprovedActivityDetailAsync(int id);

        Task<Result<IEnumerable<ActivityResponse>>> GetMyActiviesAsync(string userId);

        Task<Result<ActivityResponse>> CreateActivityAsync(
            CreateActivityRequest request,
            string useId);

        Task<Result<ActivityResponse>> UpdateActivityAsync(
            int id,
            UpdateActivityRequest request,
            string useId,
            bool isAdmin);

        Task<Result<IEnumerable<ActivityResponse>>> GetPendingActivitiesAsync();

        Task<Result<ActivityResponse>> ApproveActivityAsync(int id);

        Task<Result<ActivityResponse>> RejectActivityAsync(int id);

        Task<Result<object>> DeleteActivityAsync(int id);
    }
}


