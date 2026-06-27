namespace UniversityActivities.Api.Domain.Entities
{
    public class ActivityRegistration
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public Activity? Activity { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public AppUser? Student {  get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.Now;
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Registered;
    }
}


