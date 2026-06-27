namespace UniversityActivities.Api.Domain.Entities
{
    public enum ActivityType
    {
        Workshop = 1,
        Seminar = 2,
        Competition = 3,
        Volunteer = 4,
        ClubEvent = 5
    }

    public enum ActivityStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }

    public enum RegistrationStatus
    {
        Registered = 1,
        Attended = 2,
        Cancelled = 3,
        Absent = 4
    }
}


