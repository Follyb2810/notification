using Microsoft.AspNetCore.Mvc;
using Notification.Features.TextMessages.DTOs;
using Notification.Features.TextMessages.Interfaces;
using Notification.Infrastructure.Filters;
using Notification.Shared.Responses;

namespace Notification.Features.TextMessages.Controllers;

[ApiController]
[Route("api/webhooks/text-messages")]
[ServiceFilter(typeof(WebhookAccessFilter))]
public class TextMessageWebhookController : ControllerBase
{
    private readonly ITextMessageService _textMessageService;

    public TextMessageWebhookController(ITextMessageService textMessageService)
    {
        _textMessageService = textMessageService;
    }

    [HttpPost]
    public async Task<IActionResult> Accept(
        CreateTextMessageWebhookDto request,
        CancellationToken cancellationToken)
    {
        var result = await _textMessageService.AcceptAsync(request, cancellationToken);
        return result.ToApiResult();
    }
}
