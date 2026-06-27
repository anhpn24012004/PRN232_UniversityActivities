using System.Security.Claims;
using UniversityActivities.Api.Domain.Entities;

namespace UniversityActivities.Api.Application.Authorization
{
    public interface IActivityAuthorizationRule
    {
        bool CanEditActivity(ClaimsPrincipal user, Activity activity);
    }
}
