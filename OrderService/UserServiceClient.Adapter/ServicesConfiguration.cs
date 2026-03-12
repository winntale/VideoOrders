using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Abstractions.Clients;
using UserService.Abstractions.Options;
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

        services.AddHttpClient(UserServiceClientOptions.HttpClientName);

        services.AddScoped<IUserServiceApiClient, UserServiceApiClient>();

        return services;
    }
}