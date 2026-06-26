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

        public async Task<IEnumerable<Activity>> GetActivitiesByOrganizerAsync(string organizerId)
        {
            return await _context.Activities
                .Include(a => a.Organizer)
                .Where(a => a.OrganizerId == organizerId)
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();
        }

        public async Task AddAsync(Activity activity)
        {
            await _context.Activities.AddAsync(activity);
        }

        public void Update(Activity activity)
        {
            _context.Activities.Update(activity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}