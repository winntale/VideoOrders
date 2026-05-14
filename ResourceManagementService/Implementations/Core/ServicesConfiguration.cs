using Core.Abstractions.Operations;
using Core.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServicesConfiguration
{
    public static void ConfigureCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IResourceEstimator, ResourceEstimator>();
        services.AddScoped<IReserveResourcesOperation, ReserveResourcesOperation>();
        services.AddScoped<IReleaseResourcesOperation, ReleaseResourcesOperation>();
    }
}
