using Notification.Shared.Enums;

namespace Notification.Shared.Entities;

public class MessageDeliveryEvent : BaseEntity
{
    public Guid OutboundMessageId { get; set; }

    public OutboundMessage? OutboundMessage { get; set; }

    public MessageStatus Status { get; set; }

    public string? Detail { get; set; }
}
