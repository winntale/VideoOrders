using Core;
using Dal;
using Gateway.Configurations;
using UserServiceClient.Adapter;
using VideoArchiveClient.Adapter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services
    .AddUserServiceClients(builder.Configuration);
builder.Services
    .AddVideoServiceClients(builder.Configuration);

builder.Services.ConfigureCoreServices();

builder.Services.AddDbStorageContext(builder.Configuration);

builder.Services.ConfigureAutoMapper();

builder.Services.ConfigureSwaggerServices();

var app = builder.Build();

app.Services.ValidateMapperProfiles();

app.ConfigureSwaggerPipeline();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();