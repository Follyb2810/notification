namespace Notification.Shared.Responses;

public enum OperationErrorKind
{
    Validation = 0,
    Forbidden = 1,
    NotFound = 2
}

public class OperationResult<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public object? Errors { get; set; }

    public OperationErrorKind ErrorKind { get; set; } = OperationErrorKind.Validation;

    public static OperationResult<T> Ok(T? data = default, string message = "Success")
    {
        return new OperationResult<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static OperationResult<T> Fail(
        string message,
        OperationErrorKind errorKind = OperationErrorKind.Validation,
        object? errors = null)
    {
        return new OperationResult<T>
        {
            Success = false,
            Message = message,
            Data = default,
            ErrorKind = errorKind,
            Errors = errors
        };
    }

    public static OperationResult<T> Forbidden(string message) =>
        Fail(message, OperationErrorKind.Forbidden);

    public static OperationResult<T> NotFound(string message) =>
        Fail(message, OperationErrorKind.NotFound);
}
