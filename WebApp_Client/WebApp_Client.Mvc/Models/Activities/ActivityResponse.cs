namespace WebApp_Client.Mvc.Models.Activities;

public class ActivityResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int MaxParticipants { get; set; }

    public int Type { get; set; }

    public int Status { get; set; }

    public string OrganizerName { get; set; } = string.Empty;
}
