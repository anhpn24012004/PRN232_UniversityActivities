namespace WebApp_Client.Mvc.Models.Registrations;

public class RegistrationResponse
{
    public int Id { get; set; }

    public int ActivityId { get; set; }

    public string ActivityTitle { get; set; } = string.Empty;

    public string StudentId { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public DateTime RegisteredAt { get; set; }

    public int Status { get; set; }
}
