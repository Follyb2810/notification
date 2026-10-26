using Notification.Features.SmsService.DTOs;

namespace Notification.Features.SmsService.Interfaces;

public interface ISmsService
{
    Task<SmsSendResult> SendAsync(SmsRequest request, CancellationToken cancellationToken);
}
