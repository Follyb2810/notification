using Microsoft.EntityFrameworkCore;

namespace Notification.Extensions;

public static class DatabaseExtension
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ConnectionString");
        services.AddDbContext<Infrastructure.Persistence.ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.EnableRetryOnFailure(maxRetryCount: 3)));

        return services;
    }
}
