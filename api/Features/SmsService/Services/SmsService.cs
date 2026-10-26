using Notification.Features.SmsService.DTOs;
using Notification.Features.SmsService.Interfaces;

namespace Notification.Features.SmsService.Services;

public class SmsService : ISmsService
{
    public Task<SmsSendResult> SendAsync(SmsRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SmsSendResult
        {
            Accepted = false,
            Error = "No SMS provider is registered. Sending stays behind ISmsService until a provider is added."
        });
    }
}
