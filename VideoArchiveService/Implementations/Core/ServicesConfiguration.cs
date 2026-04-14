using AutoMapper;
using Core.Abstractions.Operations;
using Core.CoreModelsMappingProfiles;
using Core.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServicesConfiguration
{
    public static void ConfigureCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IValidateArchiveAvailabilityOperation, ValidateArchiveAvailabilityOperation>();
    }
    
    public static void ConfigureCoreProfiles(this IMapperConfigurationExpression mc)
    {
        mc.AddMaps(typeof(ValidateArchiveAvailabilityMappingProfile).Assembly);
    }
}