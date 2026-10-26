using Notification.Features.EmailService.DTOs;

namespace Notification.Features.EmailService.Interfaces;

public interface IEmailService
{
    Task<EmailSendResult> SendAsync(EmailRequest request, CancellationToken cancellationToken);
}
