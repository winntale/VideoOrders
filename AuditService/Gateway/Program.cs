using System.Text.Json.Serialization;
using Dal;
using Gateway.Configurations;

var builder = WebApplication.CreateBuilder(args);


builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.ConfigureAutoMapper();

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

app.Services.ValidateMapperProfiles();

app.ConfigureSwaggerPipeline();

app.Run();