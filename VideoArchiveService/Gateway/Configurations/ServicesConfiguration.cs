using AutoMapper;
using Core.Abstractions.Operations;
using Gateway.GatewayModelsMappingProfiles;

namespace Gateway.Configurations;

public static class ServicesConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        throw new NotImplementedException();
    }

    public static void ConfigureGatewayProfiles(this IMapperConfigurationExpression mc)
    {
        mc.AddMaps(typeof(VideoArchiveMappingProfile).Assembly);
    }
}