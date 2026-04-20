using Dal;
using Gateway.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDalServices(builder.Configuration);

builder.Services.ConfigureMassTransit(builder.Configuration);

builder.Services.ConfigureSwaggerServices();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.ConfigureSwaggerPipeline();

app.Run();