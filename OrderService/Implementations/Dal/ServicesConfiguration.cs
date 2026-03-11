using AutoMapper;
using Dal.Abstractions.Common;
using Dal.Abstractions.Repositories;
using Dal.Common;
using Dal.Context;
using Dal.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public static class ServicesConfiguration
{
    public static void AddDbStorageContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options => options
            .UseNpgsql(configuration.GetConnectionString(OrderDbContext.ConnectionDatabase))
        );

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void ConfigureDalProfiles(this IMapperConfigurationExpression mc)
    {
        //mc.AddMaps();
    }
}