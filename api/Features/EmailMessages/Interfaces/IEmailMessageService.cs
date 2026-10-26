using Notification.Features.EmailMessages.DTOs;
using Notification.Features.Messages.DTOs;
using Notification.Shared.Responses;

namespace Notification.Features.EmailMessages.Interfaces;

public interface IEmailMessageService
{
    Task<OperationResult<OutboundMessageDto>> AcceptAsync(
        CreateEmailWebhookDto request,
        CancellationToken cancellationToken);

    Task<OperationResult<OutboundMessageDto>> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<OperationResult<PagedResult<OutboundMessageDto>>> ListAsync(
        MessageListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<MessageSummaryDto>> SummarizeAsync(CancellationToken cancellationToken);
}
