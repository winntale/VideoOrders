using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserServiceClient.Abstractions.Clients;
using UserServiceClient.Abstractions.Enums;
using UserServiceClient.Abstractions.Options;
using UserServiceClient.Adapter.Clients;

namespace UserServiceClient.Adapter;

public static class ServicesConfiguration
{
    public static IServiceCollection AddUserServiceClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<UserServiceClientOptions>(
            configuration.GetSection("Services:UserService"));

        services.AddHttpClient(nameof(UserServiceApiClient));

        services.AddKeyedTransient<IUserServiceApiClient, UserServiceApiClient>(nameof(UserServiceEnum.UserServiceApiClientKey));

        return services;
    }
}