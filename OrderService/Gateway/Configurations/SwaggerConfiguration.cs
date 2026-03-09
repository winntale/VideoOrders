namespace Gateway.Configurations;

public static class SwaggerConfiguration
{
    public static void ConfigureSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(
            options =>
            {   
                options.EnableAnnotations();
            });
    }
    
    public static void ConfigureSwaggerPipeline(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}