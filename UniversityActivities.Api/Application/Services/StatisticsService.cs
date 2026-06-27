using Microsoft.EntityFrameworkCore;
using UniversityActivities.Api.Infrastructure.Data;
using UniversityActivities.Api.Application.Common;
using UniversityActivities.Api.Application.DTOs.Statistics;
using UniversityActivities.Api.Domain.Entities;
using UniversityActivities.Api.Application.Interfaces;

namespace UniversityActivities.Api.Application.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<AdminStatisticsResponse>> GetAdminStatisticsAsync()
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

            return new Result<AdminStatisticsResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Get statistics successfully",
                Data = data
            };
        }
    }
}

