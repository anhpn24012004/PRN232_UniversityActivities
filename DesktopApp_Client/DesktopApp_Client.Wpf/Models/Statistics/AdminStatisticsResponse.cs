namespace DesktopApp_Client.Wpf.Models.Statistics;

public class AdminStatisticsResponse
{
    public int TotalActivities { get; set; }
    public int ApprovedActivities { get; set; }
    public int PendingActivities { get; set; }
    public int TotalRegistrations { get; set; }
    public int ParticipatedStudents { get; set; }
}
