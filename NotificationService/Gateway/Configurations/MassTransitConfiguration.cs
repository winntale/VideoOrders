using Gateway.Consumers;
using MassTransit;

namespace Gateway.Configurations;

public static class MassTransitConfiguration
{
    public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            
            x.AddConsumer<NotificationOrderCompletedEventConsumer>();
            x.AddConsumer<NotificationOrderFailedEventConsumer>();
            x.AddConsumer<NotificationResourceReservationFailedEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqSection = configuration.GetSection("RabbitMq");

                var host = rabbitMqSection["Host"];
                var username = rabbitMqSection["Username"];
                var password = rabbitMqSection["Password"];

                cfg.Host(host, "/", h =>
                {
                    h.Username(username!);
                    h.Password(password!);
                });
                
                cfg.UseMessageRetry(r => r.Intervals(
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5)));

                cfg.UseDelayedRedelivery(r => r.Intervals(
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromMinutes(1)));

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}