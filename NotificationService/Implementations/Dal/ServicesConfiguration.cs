using Dal.Abstractions.Repositories;
using Dal.Context;
using Dal.Options;
using Dal.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dal;

public static class ServicesConfiguration
{
    public static IServiceCollection AddDalServices(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoOptions = configuration
            .GetSection("Mongo")
            .Get<MongoOptions>()!;

        services.AddSingleton(mongoOptions);
        services.AddSingleton<NotificationDbContext>();

        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }
}