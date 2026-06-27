namespace DesktopApp_Client.Wpf.Models.Activities;

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

    public string TypeText => Type switch
    {
        1 => "Workshop",
        2 => "Seminar",
        3 => "Competition",
        4 => "Volunteer",
        5 => "ClubEvent",
        _ => Type.ToString()
    };

    public string StatusText => Status switch
    {
        1 => "Pending",
        2 => "Approved",
        3 => "Rejected",
        _ => Status.ToString()
    };
}
