using Microsoft.EntityFrameworkCore;
using UniversityActivities.Api.Infrastructure.Data;
using UniversityActivities.Api.Domain.Entities;
using UniversityActivities.Api.Application.Interfaces;

namespace UniversityActivities.Api.Infrastructure.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Activity>> GetApprovedActivitiesAsync(
            string? keyword = null,
            ActivityType? type = null,
            string? location = null)
        {
            var query = _context.Activities
                .Include(a => a.Organizer)
                .Where(a => a.Status == ActivityStatus.Approved)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var searchKeyword = keyword.Trim().ToLower();

                query = query.Where(a =>
                    a.Title.ToLower().Contains(searchKeyword));
            }

            if (type.HasValue)
            {
                query = query.Where(a => a.Type == type.Value);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                var searchLocation = location.Trim().ToLower();

                query = query.Where(a =>
                    a.Location.ToLower().Contains(searchLocation));
            }

            return await query
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

        public async Task<IEnumerable<Activity>> GetPendingActivitiesAsync()
        {

            return await _context.Activities
                .Include(a => a.Organizer)
                .Where(a => a.Status == ActivityStatus.Pending)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public void Delete(Activity activity)
        {
            _context?.Activities.Remove(activity);
        }

        public async Task<IEnumerable<Activity>> GetAllActivitiesAsync()
        {
            return await _context.Activities
                .Include(a => a.Organizer)
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Activity>> GetOrganizedActivitiesAsync()
        {
            return await _context.Activities
                .Include(a => a.Organizer)
                .Where(a =>
                    a.Status == ActivityStatus.Approved &&
                    a.EndTime < DateTime.Now)
                .OrderByDescending(a => a.EndTime)
                .ToListAsync();
        }
    }
}

