using Microsoft.AspNetCore.Mvc;
using Notification.Features.EmailMessages.DTOs;
using Notification.Features.EmailMessages.Interfaces;
using Notification.Infrastructure.Filters;
using Notification.Shared.Responses;

namespace Notification.Features.EmailMessages.Controllers;

[ApiController]
[Route("api/webhooks/emails")]
[ServiceFilter(typeof(WebhookAccessFilter))]
public class EmailWebhookController : ControllerBase
{
    private readonly IEmailMessageService _emailMessageService;

    public EmailWebhookController(IEmailMessageService emailMessageService)
    {
        _emailMessageService = emailMessageService;
    }

    [HttpPost]
    public async Task<IActionResult> Accept(
        CreateEmailWebhookDto request,
        CancellationToken cancellationToken)
    {
        var result = await _emailMessageService.AcceptAsync(request, cancellationToken);
        return result.ToApiResult();
    }
}
