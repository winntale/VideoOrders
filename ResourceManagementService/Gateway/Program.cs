using Core;
using Dal;
using Dal.Context;
using Gateway.Configurations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbStorageContext(builder.Configuration);
builder.Services.ConfigureCoreServices();
builder.Services.ConfigureMassTransit(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ResourceDbContext>();
    dbContext.Database.Migrate();
}

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
