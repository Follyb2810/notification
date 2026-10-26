using Notification.Features.Messages.Interfaces;
using Notification.Features.Messages.Services;

namespace Notification.Features.Messages;

public static class MessageModule
{
    public static IServiceCollection AddMessageModule(this IServiceCollection services)
    {
        services.AddScoped<IMessageLogService, MessageLogService>();
        return services;
    }
}
