using UniversityActivities.Api.Models;

namespace UniversityActivities.Api.Repositories.Interfaces
{
    public interface IActivityRepository
    {
        Task<IEnumerable<Activity>> GetApprovedActivitiesAsync();

        Task<Activity?> GetByIdAsync(int id);

        Task<Activity?> GetApprovedActivityByIdAsync(int id);
    }
}
