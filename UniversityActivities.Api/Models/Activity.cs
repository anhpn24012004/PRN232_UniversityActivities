using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace UniversityActivities.Api.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;


        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [Range(1, 1000)]
        public int MaxParticipants { get; set; }

        public ActivityType Type { get; set; }
        public ActivityStatus Status { get; set; } = ActivityStatus.Pending;
        public string OrganizerId { get; set; } = string.Empty;
        public AppUser? Organizer { get; set; }
        public ICollection<ActivityRegistration> Registrations { get; set; }
            = new List<ActivityRegistration>();
    }
}
