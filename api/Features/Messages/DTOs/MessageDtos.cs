using Notification.Shared.Enums;

namespace Notification.Features.Messages.DTOs;

public class OutboundMessageDto
{
    public Guid Id { get; set; }

    public MessageChannel Channel { get; set; }

    public string VendorReference { get; set; } = string.Empty;

    public string Recipient { get; set; } = string.Empty;

    public string? Subject { get; set; }

    public string Body { get; set; } = string.Empty;

    public MessageStatus Status { get; set; }

    public string? ProviderName { get; set; }

    public string? ProviderMessageId { get; set; }

    public string? FailureReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? DeliveredAt { get; set; }
}

public class MessageSummaryDto
{
    public int Total { get; set; }

    public int Received { get; set; }

    public int Sending { get; set; }

    public int Sent { get; set; }

    public int Delivered { get; set; }

    public int Failed { get; set; }

    public int SentOut { get; set; }
}

public class MessageReportDto
{
    public MessageSummaryDto TextMessages { get; set; } = new();

    public MessageSummaryDto Emails { get; set; } = new();

    public MessageSummaryDto All { get; set; } = new();
}

public class MessageListQuery
{
    public MessageStatus? Status { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}

public class SendAttempt
{
    public bool Accepted { get; init; }

    public string? ProviderMessageId { get; init; }

    public string? ProviderName { get; init; }

    public string? Error { get; init; }
}
