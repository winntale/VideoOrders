using AutoMapper;
using Dal.Abstractions.Repositories;
using Dal.Clients;
using Dal.DalModelsMappingProfiles;
using Dal.HostedServices;
using Dal.Options;
using Dal.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dal;

public static class ServicesConfiguration
{
    public static IServiceCollection AddDalServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration
            .GetSection("Elasticsearch")
            .Get<ElasticsearchOptions>()!;

        services.AddSingleton(options);
        services.AddSingleton<AuditElasticsearchClient>();
        services.AddSingleton<IAuditRepository, AuditRepository>();
        services.AddHostedService<ElasticsearchIndexInitializer>();

        return services;
    }
    
    public static void ConfigureDalProfiles(this IMapperConfigurationExpression mc)
    {
        mc.AddMaps(typeof(AuditMappingProfile).Assembly);
    }
}