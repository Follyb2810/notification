using Notification.Features.Messages.DTOs;
using Notification.Shared.Enums;
using Notification.Shared.Responses;

namespace Notification.Features.Messages.Interfaces;

public interface IMessageLogService
{
    Task<OperationResult<OutboundMessageDto>> RecordAndSendAsync(
        MessageChannel channel,
        string vendorReference,
        string recipient,
        string? subject,
        string body,
        string? senderId,
        bool isHtml,
        string rawPayload,
        Func<CancellationToken, Task<SendAttempt>> send,
        CancellationToken cancellationToken);

    Task<OperationResult<OutboundMessageDto>> GetAsync(
        Guid id,
        MessageChannel channel,
        CancellationToken cancellationToken);

    Task<OperationResult<PagedResult<OutboundMessageDto>>> ListAsync(
        MessageChannel channel,
        MessageListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<MessageSummaryDto>> SummarizeAsync(
        MessageChannel? channel,
        CancellationToken cancellationToken);

    Task<OperationResult<MessageReportDto>> ReportAsync(CancellationToken cancellationToken);

    Task<OperationResult<OutboundMessageDto>> ApplyReceiptAsync(
        string? providerMessageId,
        string? vendorReference,
        MessageChannel? channel,
        bool delivered,
        string? detail,
        CancellationToken cancellationToken);
}
