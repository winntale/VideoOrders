using Microsoft.AspNetCore.Authentication.Cookies;
using VideoOrdersPortal.Auth;
using VideoOrdersPortal.Clients;
using VideoOrdersPortal.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var backends = builder.Configuration.GetSection("Backends").Get<BackendOptions>()
               ?? throw new InvalidOperationException("Backends configuration is missing.");
builder.Services.AddSingleton(backends);

builder.Services.AddHttpClient<ProcessingSystemClient>(c => c.BaseAddress = new Uri(backends.ProcessingSystemUrl));
builder.Services.AddHttpClient<UserServiceClient>(c => c.BaseAddress = new Uri(backends.UserServiceUrl));
builder.Services.AddHttpClient<VideoArchiveServiceClient>(c => c.BaseAddress = new Uri(backends.VideoArchiveServiceUrl));
builder.Services.AddHttpClient<OrderServiceClient>(c => c.BaseAddress = new Uri(backends.OrderServiceUrl));
builder.Services.AddHttpClient<NotificationServiceClient>(c => c.BaseAddress = new Uri(backends.NotificationServiceUrl));

builder.Services
    .AddAuthentication(PortalSession.CookieScheme)
    .AddCookie(PortalSession.CookieScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
