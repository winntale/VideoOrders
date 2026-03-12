using AutoMapper;
using Gateway.GatewayModelsMappingProfiles;

namespace Gateway.Configurations;

public static class ServicesConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
    }
    
    public static void ConfigureGatewayProfiles(this IMapperConfigurationExpression mc)
    {
        mc.AddMaps(typeof(ValidateUserMappingProfile));
    }
}