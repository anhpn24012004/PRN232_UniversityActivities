namespace UniversityActivities.Api.Application.Common
{
    public class Result<T>
    {
        public bool Success { get; set; }

        public int StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public object? Errors { get; set; }

        public static Result<T> Ok(T? data, string message = "Success")
        {
            return new Result<T>
            {
                Success = true,
                StatusCode = 200,
                Message = message,
                Data = data
            };
        }

        public static Result<T> Created(T? data, string message = "Created")
        {
            return new Result<T>
            {
                Success = true,
                StatusCode = 201,
                Message = message,
                Data = data
            };
        }

        public static Result<T> BadRequest(string message, object? errors = null)
        {
            return Fail(400, message, errors);
        }

        public static Result<T> Unauthorized(string message)
        {
            return Fail(401, message);
        }

        public static Result<T> Forbidden(string message)
        {
            return Fail(403, message);
        }

        public static Result<T> NotFound(string message)
        {
            return Fail(404, message);
        }

        public static Result<T> Conflict(string message)
        {
            return Fail(409, message);
        }

        public static Result<T> Fail(int statusCode, string message, object? errors = null)
        {
            return new Result<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Errors = errors
            };
        }
    }
}
