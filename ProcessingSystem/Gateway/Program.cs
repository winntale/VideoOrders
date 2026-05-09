using Gateway.Configurations;
using Gateway.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<CameraInputOptions>(builder.Configuration.GetSection("CameraInput"));
builder.Services.Configure<ArchiveStorageOptions>(builder.Configuration.GetSection("ArchiveStorage"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.ConfigureMassTransit(builder.Configuration);

var app = builder.Build();

app.UseRouting();
app.UseCors();
app.MapControllers();

app.Run();
