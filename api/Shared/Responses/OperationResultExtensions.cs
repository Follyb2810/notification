using Microsoft.AspNetCore.Mvc;

namespace Notification.Shared.Responses;

public static class OperationResultExtensions
{
    public static IActionResult ToApiResult<T>(this OperationResult<T> result)
    {
        var (statusCode, code) = result.Success
            ? (200, "SUCCESS")
            : result.ErrorKind switch
            {
                OperationErrorKind.Forbidden => (403, "FORBIDDEN"),
                OperationErrorKind.NotFound => (404, "NOT_FOUND"),
                _ => (400, "ERROR")
            };

        return new ObjectResult(
            result.Success
                ? ApiResult<T>.Ok(result.Data, result.Message, statusCode)
                : ApiResult<T>.Fail(result.Message, code, statusCode, result.Errors))
        {
            StatusCode = statusCode
        };
    }
}
