using Gateway.Options;
using Gateway.Services;

namespace Gateway.Configurations;

public static class ServicesConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ArchiveStorageOptions>(configuration.GetSection("ArchiveStorage"));
        services.AddScoped<VideoProcessingService>();
    }
}