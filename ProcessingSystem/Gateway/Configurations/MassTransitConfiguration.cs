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

            x.AddConsumer<ProcessingResourceReservedEventConsumer>();

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

                // Транзиентные ошибки (упавший канал, секундный таймаут публикации)
                // не должны сразу пихать сообщение в _error: повторим 3 раза
                // с инкрементом перед тем, как считать сбой окончательным.
                cfg.UseMessageRetry(r => r.Incremental(
                    retryLimit: 3,
                    initialInterval: TimeSpan.FromSeconds(2),
                    intervalIncrement: TimeSpan.FromSeconds(5)));

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
