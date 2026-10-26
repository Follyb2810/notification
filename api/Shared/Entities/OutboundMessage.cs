using Notification.Shared.Enums;

namespace Notification.Shared.Entities;

public class OutboundMessage : BaseEntity
{
    public MessageChannel Channel { get; set; }

    public string VendorReference { get; set; } = string.Empty;

    public string Recipient { get; set; } = string.Empty;

    public string? Subject { get; set; }

    public string Body { get; set; } = string.Empty;

    public MessageStatus Status { get; set; } = MessageStatus.Received;

    public string? ProviderName { get; set; }

    public string? ProviderMessageId { get; set; }

    public string? FailureReason { get; set; }

    public string? SenderId { get; set; }

    public bool IsHtml { get; set; }

    public string? RawPayload { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public ICollection<MessageDeliveryEvent> Events { get; set; } = [];
}
