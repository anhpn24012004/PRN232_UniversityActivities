using Microsoft.EntityFrameworkCore;
using UniversityActivities.Api.Infrastructure.Data;
using UniversityActivities.Api.Domain.Entities;
using UniversityActivities.Api.Application.Interfaces;

namespace UniversityActivities.Api.Infrastructure.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly ApplicationDbContext _context;

        public RegistrationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ActivityRegistration registration)
        {
            await _context.ActivityRegistrations.AddAsync(registration);
        }

        public async Task<int> CountRegisteredStudentsAsync(int activityId)
        {
            return await _context.ActivityRegistrations
                .CountAsync(r =>
                    r.ActivityId == activityId &&
                    r.Status == RegistrationStatus.Registered);
        }

        public async Task<ActivityRegistration?> GetByActivityAndStudentAsync(int activityId, string studentId)
        {
            return await _context.ActivityRegistrations
                .Include(r => r.Activity)
                .Include(r => r.Student)
                .FirstOrDefaultAsync(r =>
                    r.ActivityId == activityId &&
                    r.StudentId == studentId);
        }

        public async Task<ActivityRegistration?> GetByIdAsync(int id)
        {
            return await _context.ActivityRegistrations
                .Include(r => r.Activity)
                .Include(r => r.Student)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<ActivityRegistration>> GetRegistrationsByStudentAsync(string studentId)
        {
            return await _context.ActivityRegistrations
                .Include(r => r.Activity)
                .Include(r => r.Student)
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.RegisteredAt)
                .ToListAsync();
        }

        public void Update(ActivityRegistration registration)
        {
            _context.ActivityRegistrations.Update(registration);
        }

        public async Task<IEnumerable<ActivityRegistration>> GetRegistrationsByActivityAsync(int activityId)
        {
            return await _context.ActivityRegistrations
                .Include(r => r.Activity)
                .Include(r => r.Student)
                .Where(r => r.ActivityId == activityId)
                .OrderByDescending (r => r.RegisteredAt)
                .ToListAsync();
        }
    }
}


