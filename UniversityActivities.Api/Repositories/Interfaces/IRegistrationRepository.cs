using UniversityActivities.Api.Models;

namespace UniversityActivities.Api.Repositories.Interfaces
{
    public interface IRegistrationRepository
    {
        Task<ActivityRegistration?> GetByIdAsync(int id);

        Task<ActivityRegistration?> GetByActivityAndStudentAsync(
            int activityId,
            string studentId);

        Task<IEnumerable<ActivityRegistration>> GetRegistrationsByStudentAsync(
            string studentId);

        Task<int> CountRegisteredStudentsAsync(int  activityId);

        Task AddAsync(ActivityRegistration registration);

        void Update(ActivityRegistration registration);

        Task SaveChangeAsync();
    }
}
