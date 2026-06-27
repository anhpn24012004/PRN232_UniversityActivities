using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebApp_Client.Mvc.Models.Common;

namespace WebApp_Client.Mvc;

public class ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _baseUrl;

    public ApiClient(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _baseUrl = configuration["ApiSettings:BaseUrl"]?.TrimEnd('/')
            ?? throw new InvalidOperationException("ApiSettings:BaseUrl is missing.");
    }

    public Task<ApiResult<T>> GetAsync<T>(string url)
    {
        return SendAsync<object, T>(HttpMethod.Get, url, null);
    }

    public Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        return SendAsync<TRequest, TResponse>(HttpMethod.Post, url, data);
    }

    public Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest data)
    {
        return SendAsync<TRequest, TResponse>(HttpMethod.Put, url, data);
    }

    public Task<ApiResult<TResponse>> DeleteAsync<TResponse>(string url)
    {
        return SendAsync<object, TResponse>(HttpMethod.Delete, url, null);
    }

    private async Task<ApiResult<TResponse>> SendAsync<TRequest, TResponse>(
        HttpMethod method,
        string url,
        TRequest? data)
    {
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(method, _baseUrl + url);
        var token = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.Token);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (data is not null)
        {
            var json = JsonSerializer.Serialize(data, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        try
        {
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(content))
            {
                var result = JsonSerializer.Deserialize<ApiResult<TResponse>>(content, JsonOptions);
                if (result is not null)
                {
                    result.StatusCode = result.StatusCode == 0 ? (int)response.StatusCode : result.StatusCode;
                    return result;
                }
            }

            return new ApiResult<TResponse>
            {
                Success = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                Message = response.IsSuccessStatusCode ? "Success" : GetHttpMessage(response.StatusCode)
            };
        }
        catch (HttpRequestException)
        {
            return new ApiResult<TResponse>
            {
                Success = false,
                StatusCode = 0,
                Message = "Cannot connect to backend API."
            };
        }
    }

    private static string GetHttpMessage(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.Unauthorized => "Unauthorized. Please login again.",
            HttpStatusCode.Forbidden => "Forbidden. You do not have permission.",
            HttpStatusCode.NotFound => "Resource not found.",
            _ => "API request failed."
        };
    }
}
