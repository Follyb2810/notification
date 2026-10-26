using System.Text.Json;
using Notification.Features.Messages.DTOs;
using Notification.Features.Messages.Interfaces;
using Notification.Features.SmsService.DTOs;
using Notification.Features.SmsService.Interfaces;
using Notification.Features.TextMessages.DTOs;
using Notification.Features.TextMessages.Interfaces;
using Notification.Shared.Enums;
using Notification.Shared.Responses;

namespace Notification.Features.TextMessages.Services;

public class TextMessageService : ITextMessageService
{
    private readonly IMessageLogService _messageLog;
    private readonly ISmsService _smsService;

    public TextMessageService(IMessageLogService messageLog, ISmsService smsService)
    {
        _messageLog = messageLog;
        _smsService = smsService;
    }

    public Task<OperationResult<OutboundMessageDto>> AcceptAsync(
        CreateTextMessageWebhookDto request,
        CancellationToken cancellationToken)
    {
        var phoneNumber = NormalizePhone(request.PhoneNumber);
        if (phoneNumber.Count(char.IsDigit) < 7)
        {
            return Task.FromResult(OperationResult<OutboundMessageDto>.Fail(
                "Phone number is not valid."));
        }

        return _messageLog.RecordAndSendAsync(
            MessageChannel.Sms,
            request.Reference.Trim(),
            phoneNumber,
            subject: null,
            request.Message.Trim(),
            request.SenderId,
            isHtml: false,
            JsonSerializer.Serialize(request),
            async token =>
            {
                var result = await _smsService.SendAsync(new SmsRequest
                {
                    PhoneNumber = phoneNumber,
                    Message = request.Message.Trim(),
                    Reference = request.Reference.Trim(),
                    SenderId = request.SenderId
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
        _messageLog.GetAsync(id, MessageChannel.Sms, cancellationToken);

    public Task<OperationResult<PagedResult<OutboundMessageDto>>> ListAsync(
        MessageListQuery query,
        CancellationToken cancellationToken) =>
        _messageLog.ListAsync(MessageChannel.Sms, query, cancellationToken);

    public Task<OperationResult<MessageSummaryDto>> SummarizeAsync(CancellationToken cancellationToken) =>
        _messageLog.SummarizeAsync(MessageChannel.Sms, cancellationToken);

    private static string NormalizePhone(string phoneNumber)
    {
        var characters = phoneNumber
            .Trim()
            .Where(character => char.IsDigit(character) || character == '+')
            .ToArray();
        return new string(characters);
    }
}
