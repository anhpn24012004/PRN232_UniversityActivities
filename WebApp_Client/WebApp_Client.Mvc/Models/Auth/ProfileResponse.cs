namespace WebApp_Client.Mvc.Models.Auth;

public class ProfileResponse
{
    public string Id { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public string? StudentCode { get; set; }

    public string Department { get; set; } = string.Empty;

    public IList<string> Roles { get; set; } = new List<string>();
}
