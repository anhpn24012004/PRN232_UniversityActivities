using WebApp_Client.Mvc.Models.Activities;

namespace WebApp_Client.Mvc.Models.ViewModels;

public class StaffRegistrationsViewModel
{
    public IEnumerable<ActivityResponse> PendingActivities { get; set; } = Enumerable.Empty<ActivityResponse>();

    public IEnumerable<ActivityResponse> ApprovedActivities { get; set; } = Enumerable.Empty<ActivityResponse>();
}
