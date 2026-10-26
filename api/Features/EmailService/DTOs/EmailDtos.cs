namespace Notification.Features.EmailService.DTOs;

public class EmailRequest
{
    public required string To { get; init; }

    public required string Subject { get; init; }

    public required string Body { get; init; }

    public required string Reference { get; init; }

    public bool IsHtml { get; init; }
}

public class EmailSendResult
{
    public bool Accepted { get; init; }

    public string? ProviderMessageId { get; init; }

    public string? ProviderName { get; init; }

    public string? Error { get; init; }
}
