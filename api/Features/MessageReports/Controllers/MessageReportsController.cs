using Microsoft.AspNetCore.Mvc;
using Notification.Features.Messages.Interfaces;
using Notification.Infrastructure.Filters;
using Notification.Shared.Responses;

namespace Notification.Features.MessageReports.Controllers;

[ApiController]
[Route("api/message-reports")]
[ServiceFilter(typeof(WebhookAccessFilter))]
public class MessageReportsController : ControllerBase
{
    private readonly IMessageLogService _messageLog;

    public MessageReportsController(IMessageLogService messageLog)
    {
        _messageLog = messageLog;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
    {
        var result = await _messageLog.ReportAsync(cancellationToken);
        return result.ToApiResult();
    }
}
