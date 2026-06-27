namespace WebApp_Client.Mvc.Models.Common;

public class ApiResult<T>
{
    public bool Success { get; set; }

    public int StatusCode { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public object? Errors { get; set; }
}
