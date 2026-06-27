using UniversityActivities.Api.Domain.Entities;

namespace UniversityActivities.Api.Application.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(AppUser user);
    }
}


