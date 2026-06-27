using UniversityActivities.Api.Domain.Entities;

namespace UniversityActivities.Api.Application.DTOs.Activities
{
    public class ActivityResponse
    {
        public int Id {  get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Location {  get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }  

        public int MaxParticipants { get; set; }

        public ActivityType Type { get; set; }

        public ActivityStatus Status { get; set; }

        public string OrganizerName { get; set; } = string.Empty;
    }
}


