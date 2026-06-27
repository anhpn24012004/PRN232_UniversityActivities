namespace UniversityActivities.Api.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IActivityRepository Activities { get; }

        IRegistrationRepository Registrations { get; }

        Task<int> SaveChangesAsync();
    }
}
