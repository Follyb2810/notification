using Microsoft.AspNetCore.Mvc;
using Notification.Features.Messages.DTOs;
using Notification.Features.TextMessages.Interfaces;
using Notification.Infrastructure.Filters;
using Notification.Shared.Responses;

namespace Notification.Features.TextMessages.Controllers;

[ApiController]
[Route("api/text-messages")]
[ServiceFilter(typeof(WebhookAccessFilter))]
public class TextMessagesController : ControllerBase
{
    private readonly ITextMessageService _textMessageService;

    public TextMessagesController(ITextMessageService textMessageService)
    {
        _textMessageService = textMessageService;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] MessageListQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _textMessageService.ListAsync(query, cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
    {
        var result = await _textMessageService.SummarizeAsync(cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _textMessageService.GetAsync(id, cancellationToken);
        return result.ToApiResult();
    }
}
