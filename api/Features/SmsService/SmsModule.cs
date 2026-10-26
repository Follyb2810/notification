namespace Notification.Features.SmsService;

public static class SmsModule
{
    public static IServiceCollection AddSmsModule(this IServiceCollection services)
    {
        services.AddScoped<Interfaces.ISmsService, Services.SmsService>();
        return services;
    }
}
