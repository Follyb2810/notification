using Notification.Features.EmailService.DTOs;
using Notification.Features.EmailService.Interfaces;

namespace Notification.Features.EmailService.Services;

public class EmailService : IEmailService
{
    public Task<EmailSendResult> SendAsync(EmailRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new EmailSendResult
        {
            Accepted = false,
            Error = "No email provider is registered. Sending stays behind IEmailService until a provider is added."
        });
    }
}
