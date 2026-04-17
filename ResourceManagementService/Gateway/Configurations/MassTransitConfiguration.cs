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
            
            x.AddConsumer<OrderCreatedConsumer>();

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

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}