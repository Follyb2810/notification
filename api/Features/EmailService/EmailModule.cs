namespace Notification.Features.EmailService;

public static class EmailModule
{
    public static IServiceCollection AddEmailModule(this IServiceCollection services)
    {
        services.AddScoped<Interfaces.IEmailService, Services.EmailService>();
        return services;
    }
}
