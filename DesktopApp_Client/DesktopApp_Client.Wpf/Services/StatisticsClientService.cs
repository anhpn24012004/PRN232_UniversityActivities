using DesktopApp_Client.Wpf.Models.Common;
using DesktopApp_Client.Wpf.Models.Statistics;

namespace DesktopApp_Client.Wpf.Services;

public class StatisticsClientService
{
    private readonly ApiClient _apiClient = new();

    public Task<ApiResult<AdminStatisticsResponse>> GetStatisticsAsync()
    {
        return _apiClient.GetAsync<AdminStatisticsResponse>("/api/Statistics");
    }
}
