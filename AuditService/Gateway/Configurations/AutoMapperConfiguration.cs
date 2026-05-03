using AutoMapper;
using Dal;

namespace Gateway.Configurations;

public static class AutoMapperConfiguration
{
    public static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(mc =>
        {
            mc.ConfigureDalProfiles();
        });
    }

    public static void ValidateMapperProfiles(this IServiceProvider serviceProvider)
    {
        serviceProvider.GetRequiredService<IMapper>()
            .ConfigurationProvider
            .AssertConfigurationIsValid();
    }
}