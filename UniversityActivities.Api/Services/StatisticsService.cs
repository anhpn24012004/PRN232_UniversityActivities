using Microsoft.EntityFrameworkCore;
using UniversityActivities.Api.Data;
using UniversityActivities.Api.DTOs.Common;
using UniversityActivities.Api.DTOs.Statistics;
using UniversityActivities.Api.Models;
using UniversityActivities.Api.Services.Interfaces;

namespace UniversityActivities.Api.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<AdminStatisticsResponse>> GetAdminStatisticsAsync()
        {
            var data = new AdminStatisticsResponse
            {
                TotalActivities = await _context.Activities.CountAsync(),

                ApprovedActivities = await _context.Activities
                    .CountAsync(a => a.Status == ActivityStatus.Approved),

                PendingActivities = await _context.Activities
                    .CountAsync(a => a.Status == ActivityStatus.Pending),

                TotalRegistrations = await _context.ActivityRegistrations
                    .CountAsync(),

                ParticipatedStudents = await _context.ActivityRegistrations
                    .Where(r => r.Status == RegistrationStatus.Attended)
                    .Select(r => r.StudentId)
                    .Distinct()
                    .CountAsync()
            };

            return new ApiResponse<AdminStatisticsResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get statistics successfully",
                Data = data
            };
        }
    }
}