using UniversityActivities.Api.Application.Common;
using UniversityActivities.Api.Application.DTOs.Statistics;

namespace UniversityActivities.Api.Application.Interfaces
{
    public interface IStatisticsService
    {
        Task<Result<AdminStatisticsResponse>> GetAdminStatisticsAsync();
    }
}

