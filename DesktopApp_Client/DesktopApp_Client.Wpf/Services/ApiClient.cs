using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DesktopApp_Client.Wpf.Helpers;
using DesktopApp_Client.Wpf.Models.Common;

namespace DesktopApp_Client.Wpf.Services;

public class ApiClient
{
    private static readonly HttpClient HttpClient = new()
    {
        BaseAddress = new Uri(AppConfiguration.ApiBaseUrl)
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<ApiResult<T>> GetAsync<T>(string endpoint)
    {
        using var request = CreateRequest(HttpMethod.Get, endpoint);
        return await SendAsync<T>(request);
    }

    public async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload)
    {
        using var request = CreateRequest(HttpMethod.Post, endpoint);
        request.Content = JsonContent.Create(payload);
        return await SendAsync<TResponse>(request);
    }

    public async Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest payload)
    {
        using var request = CreateRequest(HttpMethod.Put, endpoint);
        request.Content = JsonContent.Create(payload);
        return await SendAsync<TResponse>(request);
    }

    public async Task<ApiResult<TResponse>> DeleteAsync<TResponse>(string endpoint)
    {
        using var request = CreateRequest(HttpMethod.Delete, endpoint);
        return await SendAsync<TResponse>(request);
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method, string endpoint)
    {
        var request = new HttpRequestMessage(method, endpoint);
        if (!string.IsNullOrWhiteSpace(TokenStorage.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.Token);
        }

        return request;
    }

    private static async Task<ApiResult<T>> SendAsync<T>(HttpRequestMessage request)
    {
        try
        {
            using var response = await HttpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            ApiResult<T>? result = null;
            if (!string.IsNullOrWhiteSpace(json))
            {
                result = JsonSerializer.Deserialize<ApiResult<T>>(json, JsonOptions);
            }

            result ??= new ApiResult<T>
            {
                Success = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                Message = response.ReasonPhrase ?? string.Empty
            };

            if (result.StatusCode == 0)
            {
                result.StatusCode = (int)response.StatusCode;
            }

            if (string.IsNullOrWhiteSpace(result.Message))
            {
                result.Message = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => "Unauthorized",
                    System.Net.HttpStatusCode.Forbidden => "Forbidden",
                    _ => response.ReasonPhrase ?? "Request failed"
                };
            }

            return result;
        }
        catch (Exception ex)
        {
            return new ApiResult<T>
            {
                Success = false,
                StatusCode = 0,
                Message = ex.Message
            };
        }
    }
}
