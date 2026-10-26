using Notification.Features.DeliveryReceipts.DTOs;
using Notification.Features.DeliveryReceipts.Interfaces;
using Notification.Features.Messages.DTOs;
using Notification.Features.Messages.Interfaces;
using Notification.Shared.Responses;

namespace Notification.Features.DeliveryReceipts.Services;

public class DeliveryReceiptService : IDeliveryReceiptService
{
    private readonly IMessageLogService _messageLog;

    public DeliveryReceiptService(IMessageLogService messageLog)
    {
        _messageLog = messageLog;
    }

    public Task<OperationResult<OutboundMessageDto>> AcceptAsync(
        DeliveryReceiptDto request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ProviderMessageId) &&
            string.IsNullOrWhiteSpace(request.VendorReference))
        {
            return Task.FromResult(OperationResult<OutboundMessageDto>.Fail(
                "A provider message id or vendor reference is required."));
        }

        return _messageLog.ApplyReceiptAsync(
            request.ProviderMessageId,
            request.VendorReference,
            request.Channel,
            request.Delivered,
            request.Detail,
            cancellationToken);
    }
}
