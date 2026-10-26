using Notification.Features.DeliveryReceipts.DTOs;
using Notification.Features.Messages.DTOs;
using Notification.Shared.Responses;

namespace Notification.Features.DeliveryReceipts.Interfaces;

public interface IDeliveryReceiptService
{
    Task<OperationResult<OutboundMessageDto>> AcceptAsync(
        DeliveryReceiptDto request,
        CancellationToken cancellationToken);
}
