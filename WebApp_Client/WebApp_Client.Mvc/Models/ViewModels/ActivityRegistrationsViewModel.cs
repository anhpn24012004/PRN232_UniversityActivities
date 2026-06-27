using WebApp_Client.Mvc.Models.Registrations;

namespace WebApp_Client.Mvc.Models.ViewModels;

public class ActivityRegistrationsViewModel
{
    public int ActivityId { get; set; }

    public bool CanManageAttendance { get; set; }

    public string ReturnUrl { get; set; } = string.Empty;

    public IEnumerable<RegistrationResponse> Registrations { get; set; } = Enumerable.Empty<RegistrationResponse>();
}
