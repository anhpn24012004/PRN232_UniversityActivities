using UniversityActivities.Api.Models;

namespace UniversityActivities.Api.Repositories.Interfaces
{
    public interface IActivityRepository
    {
        Task<IEnumerable<Activity>> GetApprovedActivitiesAsync();

        Task<IEnumerable<Activity>> GetPendingActivitiesAsync();

        Task<Activity?> GetByIdAsync(int id);

        Task<Activity?> GetApprovedActivityByIdAsync(int id);

        Task<IEnumerable<Activity>> GetActivitiesByOrganizerAsync(string  organizerId);

        Task AddAsync(Activity activity);

        void Update(Activity activity);

        Task SaveChangesAsync();
    }
}
