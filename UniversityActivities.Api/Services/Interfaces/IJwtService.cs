using UniversityActivities.Api.Models;

namespace UniversityActivities.Api.Services.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(AppUser user);
    }
}
