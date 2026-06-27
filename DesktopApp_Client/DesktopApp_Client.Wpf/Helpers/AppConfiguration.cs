using System.IO;
using System.Text.Json;

namespace DesktopApp_Client.Wpf.Helpers;

public static class AppConfiguration
{
    public static string ApiBaseUrl { get; } = LoadApiBaseUrl();

    private static string LoadApiBaseUrl()
    {
        const string fallback = "https://localhost:7226";
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        if (!File.Exists(path))
        {
            return fallback;
        }

        using var stream = File.OpenRead(path);
        using var document = JsonDocument.Parse(stream);

        if (document.RootElement.TryGetProperty("ApiSettings", out var apiSettings) &&
            apiSettings.TryGetProperty("BaseUrl", out var baseUrlElement))
        {
            var baseUrl = baseUrlElement.GetString();
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                return baseUrl.TrimEnd('/');
            }
        }

        return fallback;
    }
}
