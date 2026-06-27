using UniversityActivities.Api.Domain.Entities;

namespace UniversityActivities.Api.Application.Interfaces
{
    public interface IActivityRepository
    {
        Task<IEnumerable<Activity>> GetApprovedActivitiesAsync(
            string? keyword = null,
            ActivityType? type = null,
            string? location = null);

        Task<IEnumerable<Activity>> GetAllActivitiesAsync();

        Task<IEnumerable<Activity>> GetOrganizedActivitiesAsync();

        Task<IEnumerable<Activity>> GetPendingActivitiesAsync();

        Task<Activity?> GetByIdAsync(int id);

        Task<Activity?> GetApprovedActivityByIdAsync(int id);

        Task<IEnumerable<Activity>> GetActivitiesByOrganizerAsync(string  organizerId);

        Task AddAsync(Activity activity);

        void Update(Activity activity);

        void Delete(Activity activity);
    }
}


