using Microsoft.AspNetCore.Mvc;
using Notification.Features.EmailMessages.Interfaces;
using Notification.Features.Messages.DTOs;
using Notification.Infrastructure.Filters;
using Notification.Shared.Responses;

namespace Notification.Features.EmailMessages.Controllers;

[ApiController]
[Route("api/emails")]
[ServiceFilter(typeof(WebhookAccessFilter))]
public class EmailMessagesController : ControllerBase
{
    private readonly IEmailMessageService _emailMessageService;

    public EmailMessagesController(IEmailMessageService emailMessageService)
    {
        _emailMessageService = emailMessageService;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] MessageListQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _emailMessageService.ListAsync(query, cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
    {
        var result = await _emailMessageService.SummarizeAsync(cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _emailMessageService.GetAsync(id, cancellationToken);
        return result.ToApiResult();
    }
}
