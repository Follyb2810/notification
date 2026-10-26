using Microsoft.EntityFrameworkCore;
using Notification.Features.Messages.DTOs;
using Notification.Features.Messages.Interfaces;
using Notification.Features.Messages.Mappings;
using Notification.Infrastructure.Persistence;
using Notification.Shared.Entities;
using Notification.Shared.Enums;
using Notification.Shared.Responses;

namespace Notification.Features.Messages.Services;

public class MessageLogService : IMessageLogService
{
    private static readonly TimeSpan StaleSendWindow = TimeSpan.FromMinutes(5);

    private readonly ApplicationDbContext _context;
    private readonly ILogger<MessageLogService> _logger;

    public MessageLogService(ApplicationDbContext context, ILogger<MessageLogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OperationResult<OutboundMessageDto>> RecordAndSendAsync(
        MessageChannel channel,
        string vendorReference,
        string recipient,
        string? subject,
        string body,
        string? senderId,
        bool isHtml,
        string rawPayload,
        Func<CancellationToken, Task<SendAttempt>> send,
        CancellationToken cancellationToken)
    {
        var message = await FindByReferenceAsync(channel, vendorReference, cancellationToken);
        if (message is null)
        {
            message = await InsertReceivedAsync(
                channel,
                vendorReference,
                recipient,
                subject,
                body,
                senderId,
                isHtml,
                rawPayload,
                cancellationToken);
        }

        if (IsFinished(message.Status))
        {
            return OperationResult<OutboundMessageDto>.Ok(
                message.ToDto(),
                "Message already recorded.");
        }

        var claimed = await ClaimForSendAsync(message.Id, cancellationToken);
        if (!claimed)
        {
            var current = await _context.OutboundMessages
                .AsNoTracking()
                .FirstAsync(x => x.Id == message.Id, cancellationToken);

            return OperationResult<OutboundMessageDto>.Ok(
                current.ToDto(),
                "Message is already being sent.");
        }

        SendAttempt attempt;
        try
        {
            attempt = await send(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Dispatch failed for {Channel} reference {Reference}", channel, vendorReference);
            attempt = new SendAttempt
            {
                Accepted = false,
                Error = exception.Message
            };
        }

        var updated = await ApplySendAttemptAsync(message.Id, attempt, cancellationToken);
        return OperationResult<OutboundMessageDto>.Ok(
            updated.ToDto(),
            updated.Status == MessageStatus.Sent
                ? "Message saved and sent."
                : "Message saved. Delivery to the recipient failed.");
    }

    public async Task<OperationResult<OutboundMessageDto>> GetAsync(
        Guid id,
        MessageChannel channel,
        CancellationToken cancellationToken)
    {
        var message = await _context.OutboundMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.Channel == channel, cancellationToken);

        if (message is null)
        {
            return OperationResult<OutboundMessageDto>.NotFound("Message was not found.");
        }

        return OperationResult<OutboundMessageDto>.Ok(message.ToDto());
    }

    public async Task<OperationResult<PagedResult<OutboundMessageDto>>> ListAsync(
        MessageChannel channel,
        MessageListQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        var messages = _context.OutboundMessages
            .AsNoTracking()
            .Where(x => x.Channel == channel);

        if (query.Status.HasValue)
        {
            messages = messages.Where(x => x.Status == query.Status.Value);
        }

        var total = await messages.CountAsync(cancellationToken);
        var items = await messages
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return OperationResult<PagedResult<OutboundMessageDto>>.Ok(new PagedResult<OutboundMessageDto>
        {
            Items = items.Select(x => x.ToDto()).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<OperationResult<MessageSummaryDto>> SummarizeAsync(
        MessageChannel? channel,
        CancellationToken cancellationToken)
    {
        var rows = await CountRowsAsync(channel, cancellationToken);
        return OperationResult<MessageSummaryDto>.Ok(ToSummary(rows.Select(x => (x.Status, x.Count))));
    }

    public async Task<OperationResult<MessageReportDto>> ReportAsync(CancellationToken cancellationToken)
    {
        var rows = await CountRowsAsync(null, cancellationToken);
        return OperationResult<MessageReportDto>.Ok(new MessageReportDto
        {
            TextMessages = ToSummary(rows.Where(x => x.Channel == MessageChannel.Sms).Select(x => (x.Status, x.Count))),
            Emails = ToSummary(rows.Where(x => x.Channel == MessageChannel.Email).Select(x => (x.Status, x.Count))),
            All = ToSummary(rows.Select(x => (x.Status, x.Count)))
        });
    }

    public async Task<OperationResult<OutboundMessageDto>> ApplyReceiptAsync(
        string? providerMessageId,
        string? vendorReference,
        MessageChannel? channel,
        bool delivered,
        string? detail,
        CancellationToken cancellationToken)
    {
        var message = await FindForReceiptAsync(providerMessageId, vendorReference, channel, cancellationToken);
        if (message is null)
        {
            return OperationResult<OutboundMessageDto>.NotFound("Message was not found for this delivery receipt.");
        }

        if (message.Status == MessageStatus.Delivered)
        {
            await AddEventAsync(
                message.Id,
                MessageStatus.Delivered,
                delivered
                    ? "Duplicate delivery receipt."
                    : "Ignored a failure receipt because the message is already delivered.",
                cancellationToken);

            return OperationResult<OutboundMessageDto>.Ok(
                message.ToDto(),
                "Message is already delivered.");
        }

        var now = DateTime.UtcNow;
        message.Status = delivered ? MessageStatus.Delivered : MessageStatus.Failed;
        message.UpdatedAt = now;
        message.FailureReason = delivered ? null : Trim(detail, 2000);
        if (delivered)
        {
            message.DeliveredAt = now;
        }

        _context.MessageDeliveryEvents.Add(new MessageDeliveryEvent
        {
            Id = Guid.NewGuid(),
            OutboundMessageId = message.Id,
            Status = message.Status,
            Detail = Trim(detail, 2000),
            CreatedAt = now,
            UpdatedAt = now
        });

        await _context.SaveChangesAsync(cancellationToken);
        return OperationResult<OutboundMessageDto>.Ok(message.ToDto(), "Delivery receipt saved.");
    }

    private async Task<OutboundMessage?> FindByReferenceAsync(
        MessageChannel channel,
        string vendorReference,
        CancellationToken cancellationToken)
    {
        return await _context.OutboundMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Channel == channel && x.VendorReference == vendorReference,
                cancellationToken);
    }

    private async Task<OutboundMessage> InsertReceivedAsync(
        MessageChannel channel,
        string vendorReference,
        string recipient,
        string? subject,
        string body,
        string? senderId,
        bool isHtml,
        string rawPayload,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var message = new OutboundMessage
        {
            Id = Guid.NewGuid(),
            Channel = channel,
            VendorReference = vendorReference,
            Recipient = recipient,
            Subject = subject,
            Body = body,
            SenderId = senderId,
            IsHtml = isHtml,
            RawPayload = rawPayload,
            Status = MessageStatus.Received,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.OutboundMessages.Add(message);
        _context.MessageDeliveryEvents.Add(new MessageDeliveryEvent
        {
            Id = Guid.NewGuid(),
            OutboundMessageId = message.Id,
            Status = MessageStatus.Received,
            Detail = "Accepted from vendor webhook.",
            CreatedAt = now,
            UpdatedAt = now
        });

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return message;
        }
        catch (DbUpdateException)
        {
            _context.ChangeTracker.Clear();
            var existing = await FindByReferenceAsync(channel, vendorReference, cancellationToken);
            if (existing is null)
            {
                throw;
            }

            return existing;
        }
    }

    private async Task<bool> ClaimForSendAsync(Guid id, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var staleBefore = now.Subtract(StaleSendWindow);
        var claimed = await _context.OutboundMessages
            .Where(x => x.Id == id &&
                        (x.Status == MessageStatus.Received ||
                         (x.Status == MessageStatus.Sending && x.UpdatedAt < staleBefore)))
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(x => x.Status, MessageStatus.Sending)
                    .SetProperty(x => x.UpdatedAt, now),
                cancellationToken);

        if (claimed == 0)
        {
            return false;
        }

        _context.ChangeTracker.Clear();
        await AddEventAsync(id, MessageStatus.Sending, "Dispatch started.", cancellationToken);
        return true;
    }

    private async Task<OutboundMessage> ApplySendAttemptAsync(
        Guid id,
        SendAttempt attempt,
        CancellationToken cancellationToken)
    {
        var message = await _context.OutboundMessages.FirstAsync(x => x.Id == id, cancellationToken);
        var now = DateTime.UtcNow;
        message.ProviderName = Trim(attempt.ProviderName, 100);
        message.ProviderMessageId = Trim(attempt.ProviderMessageId, 200);
        message.UpdatedAt = now;

        if (attempt.Accepted)
        {
            message.Status = MessageStatus.Sent;
            message.SentAt = now;
            message.FailureReason = null;
        }
        else
        {
            message.Status = MessageStatus.Failed;
            message.FailureReason = Trim(attempt.Error, 2000) ?? "The gateway rejected the message.";
        }

        _context.MessageDeliveryEvents.Add(new MessageDeliveryEvent
        {
            Id = Guid.NewGuid(),
            OutboundMessageId = message.Id,
            Status = message.Status,
            Detail = attempt.Accepted
                ? "Gateway accepted the message."
                : message.FailureReason,
            CreatedAt = now,
            UpdatedAt = now
        });

        await _context.SaveChangesAsync(cancellationToken);
        return message;
    }

    private async Task AddEventAsync(
        Guid messageId,
        MessageStatus status,
        string? detail,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        _context.MessageDeliveryEvents.Add(new MessageDeliveryEvent
        {
            Id = Guid.NewGuid(),
            OutboundMessageId = messageId,
            Status = status,
            Detail = Trim(detail, 2000),
            CreatedAt = now,
            UpdatedAt = now
        });
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<OutboundMessage?> FindForReceiptAsync(
        string? providerMessageId,
        string? vendorReference,
        MessageChannel? channel,
        CancellationToken cancellationToken)
    {
        var messages = _context.OutboundMessages.AsQueryable();

        if (!string.IsNullOrWhiteSpace(providerMessageId))
        {
            var match = await messages.FirstOrDefaultAsync(
                x => x.ProviderMessageId == providerMessageId,
                cancellationToken);
            if (match is not null)
            {
                return match;
            }
        }

        if (string.IsNullOrWhiteSpace(vendorReference))
        {
            return null;
        }

        var byReference = messages.Where(x => x.VendorReference == vendorReference);
        if (channel.HasValue)
        {
            byReference = byReference.Where(x => x.Channel == channel.Value);
        }

        return await byReference.FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<List<StatusCount>> CountRowsAsync(
        MessageChannel? channel,
        CancellationToken cancellationToken)
    {
        var messages = _context.OutboundMessages.AsNoTracking();
        if (channel.HasValue)
        {
            messages = messages.Where(x => x.Channel == channel.Value);
        }

        return await messages
            .GroupBy(x => new { x.Channel, x.Status })
            .Select(group => new StatusCount(group.Key.Channel, group.Key.Status, group.Count()))
            .ToListAsync(cancellationToken);
    }

    private static MessageSummaryDto ToSummary(IEnumerable<(MessageStatus Status, int Count)> rows)
    {
        var map = rows
            .GroupBy(x => x.Status)
            .ToDictionary(group => group.Key, group => group.Sum(x => x.Count));

        int Count(MessageStatus status) => map.TryGetValue(status, out var count) ? count : 0;

        var sent = Count(MessageStatus.Sent);
        var delivered = Count(MessageStatus.Delivered);
        return new MessageSummaryDto
        {
            Received = Count(MessageStatus.Received),
            Sending = Count(MessageStatus.Sending),
            Sent = sent,
            Delivered = delivered,
            Failed = Count(MessageStatus.Failed),
            SentOut = sent + delivered,
            Total = map.Values.Sum()
        };
    }

    private static bool IsFinished(MessageStatus status) =>
        status is MessageStatus.Sent or MessageStatus.Delivered or MessageStatus.Failed;

    private static string? Trim(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
    }

    private sealed record StatusCount(MessageChannel Channel, MessageStatus Status, int Count);
}
