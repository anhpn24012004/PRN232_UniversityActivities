namespace DesktopApp_Client.Wpf.Models.Users;

public class CreateUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? StudentCode { get; set; }
    public string Department { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
