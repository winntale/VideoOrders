using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VideoArchiveClient.Abstractions.Clients;
using VideoArchiveClient.Abstractions.Enums;
using VideoArchiveClient.Abstractions.Options;
using VideoArchiveClient.Adapter.Clients;

namespace VideoArchiveClient.Adapter;

public static class ServicesConfiguration
{
    public static IServiceCollection AddVideoServiceClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<VideoArchiveServiceClientOptions>(
            configuration.GetSection("Services:VideoArchiveService"));

        services.AddHttpClient(nameof(VideoArchiveServiceApiClient));

        services.AddKeyedTransient<IVideoArchiveServiceApiClient, VideoArchiveServiceApiClient>(
            nameof(VideoArchiveServiceEnum.VideoArchiveServiceApiClientKey));

        return services;
    }
}