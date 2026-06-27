using Microsoft.AspNetCore.Authorization;
using UniversityActivities.Api.Application.Authorization;
using UniversityActivities.Api.Domain.Entities;

namespace UniversityActivities.Api.Api.Authorization
{
    public class CanEditOwnActivityHandler
        : AuthorizationHandler<CanEditOwnActivityRequirement, Activity>
    {
        private readonly IActivityAuthorizationRule _activityAuthorizationRule;

        public CanEditOwnActivityHandler(
            IActivityAuthorizationRule activityAuthorizationRule)
        {
            _activityAuthorizationRule = activityAuthorizationRule;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanEditOwnActivityRequirement requirement,
            Activity resource)
        {
            if (_activityAuthorizationRule.CanEditActivity(context.User, resource))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
