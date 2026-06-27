using UniversityActivities.Api.Domain.Entities;

namespace UniversityActivities.Api.Application.Interfaces
{
    public interface IRegistrationRepository
    {
        Task<ActivityRegistration?> GetByIdAsync(int id);

        Task<ActivityRegistration?> GetByActivityAndStudentAsync(
            int activityId,
            string studentId);

        Task<IEnumerable<ActivityRegistration>> GetRegistrationsByStudentAsync(
            string studentId);

        Task<IEnumerable<ActivityRegistration>> GetRegistrationsByActivityAsync(
            int activityId);

        Task<int> CountRegisteredStudentsAsync(int  activityId);

        Task AddAsync(ActivityRegistration registration);

        void Update(ActivityRegistration registration);
    }
}


