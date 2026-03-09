using AutoMapper;
using Core;
using Dal;

namespace Gateway.Configurations;

public static class AutoMapperConfiguration
{
    public static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(mc =>
        {
            mc.ConfigureGatewayProfiles();
            mc.ConfigureDalProfiles();
            mc.ConfigureCoreProfiles();
        });
    }

    public static void ValidateMapperProfiles(this IServiceProvider serviceProvider)
    {
        serviceProvider.GetRequiredService<IMapper>()
            .ConfigurationProvider
            .AssertConfigurationIsValid();
    }
}