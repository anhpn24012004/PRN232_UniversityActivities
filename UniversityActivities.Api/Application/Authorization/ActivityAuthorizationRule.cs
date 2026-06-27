using System.Security.Claims;
using UniversityActivities.Api.Domain.Entities;

namespace UniversityActivities.Api.Application.Authorization
{
    public class ActivityAuthorizationRule : IActivityAuthorizationRule
    {
        public bool CanEditActivity(ClaimsPrincipal user, Activity activity)
        {
            if (user.IsInRole("Admin"))
            {
                return true;
            }

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            return user.IsInRole("Organizer") &&
                !string.IsNullOrWhiteSpace(userId) &&
                activity.OrganizerId == userId;
        }
    }
}
