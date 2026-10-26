using Microsoft.AspNetCore.Mvc;
using Notification.Features.DeliveryReceipts.DTOs;
using Notification.Features.DeliveryReceipts.Interfaces;
using Notification.Infrastructure.Filters;
using Notification.Shared.Responses;

namespace Notification.Features.DeliveryReceipts.Controllers;

[ApiController]
[Route("api/webhooks/delivery-receipts")]
[ServiceFilter(typeof(WebhookAccessFilter))]
public class DeliveryReceiptController : ControllerBase
{
    private readonly IDeliveryReceiptService _deliveryReceiptService;

    public DeliveryReceiptController(IDeliveryReceiptService deliveryReceiptService)
    {
        _deliveryReceiptService = deliveryReceiptService;
    }

    [HttpPost]
    public async Task<IActionResult> Accept(
        DeliveryReceiptDto request,
        CancellationToken cancellationToken)
    {
        var result = await _deliveryReceiptService.AcceptAsync(request, cancellationToken);
        return result.ToApiResult();
    }
}
