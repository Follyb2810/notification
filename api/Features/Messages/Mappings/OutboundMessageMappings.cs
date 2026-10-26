using Notification.Features.Messages.DTOs;
using Notification.Shared.Entities;

namespace Notification.Features.Messages.Mappings;

public static class OutboundMessageMappings
{
    public static OutboundMessageDto ToDto(this OutboundMessage message)
    {
        return new OutboundMessageDto
        {
            Id = message.Id,
            Channel = message.Channel,
            VendorReference = message.VendorReference,
            Recipient = message.Recipient,
            Subject = message.Subject,
            Body = message.Body,
            Status = message.Status,
            ProviderName = message.ProviderName,
            ProviderMessageId = message.ProviderMessageId,
            FailureReason = message.FailureReason,
            CreatedAt = message.CreatedAt,
            SentAt = message.SentAt,
            DeliveredAt = message.DeliveredAt
        };
    }
}
