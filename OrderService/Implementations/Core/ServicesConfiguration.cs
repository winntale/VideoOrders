using AutoMapper;
using Core.Abstractions.Operations;
using Core.Abstractions.Services;
using Core.CoreModelsMappingProfiles;
using Core.Operations;
using Core.Options;
using Core.Resolvers;
using Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServicesConfiguration
{
    public static void ConfigureCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICreateOrderOperation, CreateOrderOperation>();
        services.AddScoped<IGetOrderByIdOperation, GetOrderByIdOperation>();
        services.AddScoped<IChangeOrderStatusOperation, ChangeOrderStatusOperation>();
        services.AddScoped<IDownloadArchiveFileOperation, DownloadArchiveFileOperation>();
        services.AddScoped<IStreamArchiveFileOperation, StreamArchiveFileOperation>();

        services.AddSingleton(new ArchiveStorageOptions
        {
            RootPath = configuration.GetSection("ArchiveStorage")["RootPath"]!
        });

        services.AddSingleton<IArchiveFileStorage, ArchiveFileStorage>();
        services.AddScoped<IArchiveFileAccessService, ArchiveFileAccessService>();
        services.AddScoped<ArchiveFileOperationResolver, ArchiveFileOperationResolver>();
    }

    public static void ConfigureCoreProfiles(this IMapperConfigurationExpression mc)
    {
        mc.AddMaps(typeof(CreateOrderOperationMappingProfile).Assembly);
    }
}