using Microsoft.EntityFrameworkCore;
using UniversityActivities.Api.Data;
using UniversityActivities.Api.Models;
using UniversityActivities.Api.Repositories.Interfaces;

namespace UniversityActivities.Api.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Activity>> GetApprovedActivitiesAsync()
        {
            return await _context.Activities
                .Include(a => a.Organizer)
                .Where(a => a.Status == ActivityStatus.Approved)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<Activity?> GetByIdAsync(int id)
        {
            return await _context.Activities
                .Include(a => a.Organizer)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Activity?> GetApprovedActivityByIdAsync(int id)
        {
            return await _context.Activities
                .Include(a => a.Organizer)
                .FirstOrDefaultAsync(a =>
                    a.Id == id &&
                    a.Status == ActivityStatus.Approved);
        }
    }
}
