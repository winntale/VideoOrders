using Dal.Abstractions.Common;
using Dal.Abstractions.Repositories;
using Dal.Common;
using Dal.Context;
using Dal.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dal;

public static class ServicesConfiguration
{
    public static void AddDbStorageContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ResourceDbContext>(options => options
            .UseNpgsql(configuration.GetConnectionString(ResourceDbContext.ConnectionDatabase)));

        services.AddScoped<IResourceRepository, ResourceRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
