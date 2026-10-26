using Notification.Features.TextMessages.Interfaces;
using Notification.Features.TextMessages.Services;

namespace Notification.Features.TextMessages;

public static class TextMessageModule
{
    public static IServiceCollection AddTextMessageModule(this IServiceCollection services)
    {
        services.AddScoped<ITextMessageService, TextMessageService>();
        return services;
    }
}
