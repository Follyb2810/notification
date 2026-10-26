using Notification.Features.DeliveryReceipts.Interfaces;
using Notification.Features.DeliveryReceipts.Services;

namespace Notification.Features.DeliveryReceipts;

public static class DeliveryReceiptModule
{
    public static IServiceCollection AddDeliveryReceiptModule(this IServiceCollection services)
    {
        services.AddScoped<IDeliveryReceiptService, DeliveryReceiptService>();
        return services;
    }
}
