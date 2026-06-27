using UniversityActivities.Api.Application.DTOs.Activities;
using UniversityActivities.Api.Domain.Entities;

namespace UniversityActivities.Api.Application.Mappings
{
    public static class ActivityMappingExtensions
    {
        public static ActivityResponse ToActivityResponse(this Activity activity)
        {
            return new ActivityResponse
            {
                Id = activity.Id,
                Title = activity.Title,
                Description = activity.Description,
                Location = activity.Location,
                StartTime = activity.StartTime,
                EndTime = activity.EndTime,
                MaxParticipants = activity.MaxParticipants,
                Type = activity.Type,
                Status = activity.Status,
                OrganizerName = activity.Organizer?.FullName ?? string.Empty
            };
        }
    }
}
