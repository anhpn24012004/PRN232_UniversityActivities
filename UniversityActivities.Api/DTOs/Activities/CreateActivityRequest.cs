using System.ComponentModel.DataAnnotations;
using UniversityActivities.Api.Models;

namespace UniversityActivities.Api.DTOs.Activities
{
    public class CreateActivityRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Location {  get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        [Range(1, 1000)]
        public int MaxParticipants { get; set; }

        public ActivityType Type { get; set; }
    }
}
