using Notification.Features.EmailMessages.Interfaces;
using Notification.Features.EmailMessages.Services;

namespace Notification.Features.EmailMessages;

public static class EmailMessageModule
{
    public static IServiceCollection AddEmailMessageModule(this IServiceCollection services)
    {
        services.AddScoped<IEmailMessageService, EmailMessageService>();
        return services;
    }
}
