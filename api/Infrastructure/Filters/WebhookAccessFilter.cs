using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Notification.Shared.Responses;

namespace Notification.Infrastructure.Filters;

public class WebhookAccessFilter : IAsyncActionFilter
{
    public const string HeaderName = "X-Notification-Secret";

    private readonly IConfiguration _configuration;

    public WebhookAccessFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var expected = _configuration["Webhooks:Secret"];
        if (string.IsNullOrWhiteSpace(expected))
        {
            context.Result = new ObjectResult(
                ApiResult<object>.Fail(
                    "Webhook secret is not configured.",
                    "FORBIDDEN",
                    StatusCodes.Status403Forbidden))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var provided) ||
            !FixedEquals(expected, provided.ToString()))
        {
            context.Result = new ObjectResult(
                ApiResult<object>.Fail(
                    "Webhook secret is invalid.",
                    "FORBIDDEN",
                    StatusCodes.Status403Forbidden))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }

        await next();
    }

    private static bool FixedEquals(string expected, string provided)
    {
        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        var providedBytes = Encoding.UTF8.GetBytes(provided);
        if (expectedBytes.Length != providedBytes.Length)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
    }
}
