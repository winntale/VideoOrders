using AutoMapper;
using Dal.Abstractions.Common;
using Dal.Abstractions.Enums;
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
        services.AddDbContext<VideoArchiveDbContext>(options => options
            .UseNpgsql(configuration.GetConnectionString(nameof(VideoArchiveDbContextEnum.VideoArchives)))
        );
        
        services.AddScoped<ICameraRepository, CameraRepository>();
        services.AddScoped<IVideoSegmentRepository, VideoSegmentRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void ConfigureDalProfiles(this IMapperConfigurationExpression mc)
    {
        //mc.AddMaps
    }
}