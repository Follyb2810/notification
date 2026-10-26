using System.Net.Mail;
using System.Text.Json;
using Notification.Features.EmailMessages.DTOs;
using Notification.Features.EmailMessages.Interfaces;
using Notification.Features.EmailService.DTOs;
using Notification.Features.EmailService.Interfaces;
using Notification.Features.Messages.DTOs;
using Notification.Features.Messages.Interfaces;
using Notification.Shared.Enums;
using Notification.Shared.Responses;

namespace Notification.Features.EmailMessages.Services;

public class EmailMessageService : IEmailMessageService
{
    private readonly IMessageLogService _messageLog;
    private readonly IEmailService _emailService;

    public EmailMessageService(IMessageLogService messageLog, IEmailService emailService)
    {
        _messageLog = messageLog;
        _emailService = emailService;
    }

    public Task<OperationResult<OutboundMessageDto>> AcceptAsync(
        CreateEmailWebhookDto request,
        CancellationToken cancellationToken)
    {
        var recipient = request.To.Trim();
        if (!MailAddress.TryCreate(recipient, out _))
        {
            return Task.FromResult(OperationResult<OutboundMessageDto>.Fail(
                "Email address is not valid."));
        }

        var subject = request.Subject.Trim();
        var body = request.Body.Trim();

        return _messageLog.RecordAndSendAsync(
            MessageChannel.Email,
            request.Reference.Trim(),
            recipient,
            subject,
            body,
            senderId: null,
            request.IsHtml,
            JsonSerializer.Serialize(request),
            async token =>
            {
                var result = await _emailService.SendAsync(new EmailRequest
                {
                    To = recipient,
                    Subject = subject,
                    Body = body,
                    Reference = request.Reference.Trim(),
                    IsHtml = request.IsHtml
                }, token);

                return new SendAttempt
                {
                    Accepted = result.Accepted,
                    ProviderMessageId = result.ProviderMessageId,
                    ProviderName = result.ProviderName,
                    Error = result.Error
                };
            },
            cancellationToken);
    }

    public Task<OperationResult<OutboundMessageDto>> GetAsync(Guid id, CancellationToken cancellationToken) =>
        _messageLog.GetAsync(id, MessageChannel.Email, cancellationToken);

    public Task<OperationResult<PagedResult<OutboundMessageDto>>> ListAsync(
        MessageListQuery query,
        CancellationToken cancellationToken) =>
        _messageLog.ListAsync(MessageChannel.Email, query, cancellationToken);

    public Task<OperationResult<MessageSummaryDto>> SummarizeAsync(CancellationToken cancellationToken) =>
        _messageLog.SummarizeAsync(MessageChannel.Email, cancellationToken);
}
