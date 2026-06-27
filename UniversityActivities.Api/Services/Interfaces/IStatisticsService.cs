using UniversityActivities.Api.DTOs.Common;
using UniversityActivities.Api.DTOs.Statistics;

namespace UniversityActivities.Api.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<ApiResponse<AdminStatisticsResponse>> GetAdminStatisticsAsync();
    }
}