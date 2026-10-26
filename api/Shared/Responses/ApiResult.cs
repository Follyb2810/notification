namespace Notification.Shared.Responses;

public class ApiResult<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public object? Errors { get; set; }

    public string? Code { get; set; }

    public int StatusCode { get; set; }

    public static ApiResult<T> Ok(
        T? data,
        string message = "Request successful",
        int statusCode = 200)
    {
        return new ApiResult<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ApiResult<T> Fail(
        string message,
        string code = "ERROR",
        int statusCode = 400,
        object? errors = null)
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = message,
            Code = code,
            StatusCode = statusCode,
            Errors = errors
        };
    }
}
