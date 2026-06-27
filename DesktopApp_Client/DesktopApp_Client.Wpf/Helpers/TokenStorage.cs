namespace DesktopApp_Client.Wpf.Helpers;

public static class TokenStorage
{
    public static string? Token { get; private set; }
    public static string? Email { get; private set; }
    public static string? FullName { get; private set; }
    public static IList<string> Roles { get; private set; } = new List<string>();
    public static bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

    public static bool IsInRole(string role)
    {
        return Roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
    }

    public static void Set(string token, string email, string fullName, IList<string> roles)
    {
        Token = token;
        Email = email;
        FullName = fullName;
        Roles = roles;
    }

    public static void Clear()
    {
        Token = null;
        Email = null;
        FullName = null;
        Roles = new List<string>();
    }
}
