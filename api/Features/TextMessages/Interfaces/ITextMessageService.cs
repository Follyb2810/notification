using Notification.Features.Messages.DTOs;
using Notification.Features.TextMessages.DTOs;
using Notification.Shared.Responses;

namespace Notification.Features.TextMessages.Interfaces;

public interface ITextMessageService
{
    Task<OperationResult<OutboundMessageDto>> AcceptAsync(
        CreateTextMessageWebhookDto request,
        CancellationToken cancellationToken);

    Task<OperationResult<OutboundMessageDto>> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<OperationResult<PagedResult<OutboundMessageDto>>> ListAsync(
        MessageListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<MessageSummaryDto>> SummarizeAsync(CancellationToken cancellationToken);
}
