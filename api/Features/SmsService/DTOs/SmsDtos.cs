namespace Notification.Features.SmsService.DTOs;

public class SmsRequest
{
    public required string PhoneNumber { get; init; }

    public required string Message { get; init; }

    public required string Reference { get; init; }

    public string? SenderId { get; init; }
}

public class SmsSendResult
{
    public bool Accepted { get; init; }

    public string? ProviderMessageId { get; init; }

    public string? ProviderName { get; init; }

    public string? Error { get; init; }
}
