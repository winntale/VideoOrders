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
        services.AddScoped<ICreateOrderOperation, CreateOrderOperation>();
        services.AddScoped<IGetOrderByIdOperation, GetOrderByIdOperation>();
        services.AddScoped<IChangeOrderStatusOperation, ChangeOrderStatusOperation>();
    }

    public static void ConfigureCoreProfiles(this IMapperConfigurationExpression mc)
    {
        mc.AddMaps(typeof(CreateOrderOperationMappingProfile).Assembly);
    }
}