using UniversityActivities.Api.Application.Interfaces;
using UniversityActivities.Api.Infrastructure.Data;

namespace UniversityActivities.Api.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(
            ApplicationDbContext context,
            IActivityRepository activities,
            IRegistrationRepository registrations)
        {
            _context = context;
            Activities = activities;
            Registrations = registrations;
        }

        public IActivityRepository Activities { get; }

        public IRegistrationRepository Registrations { get; }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
