using System.Net.Http.Json;
using VideoOrdersPortal.Models;

namespace VideoOrdersPortal.Clients;

public sealed record ProcessingCameraFile(Guid Id, string DisplayName, long FileSize);

public sealed record NotificationDto(
    Guid Id,
    Guid OrderId,
    int Type,
    string Message,
    DateTimeOffset CreatedAtUtc);

public sealed class NotificationServiceClient(HttpClient http)
{
    public async Task<IReadOnlyList<NotificationDto>> ListByOrderIdsAsync(
        IReadOnlyCollection<Guid> orderIds,
        CancellationToken ct)
    {
        if (orderIds.Count == 0) return Array.Empty<NotificationDto>();

        var query = string.Join("&", orderIds.Select(id => $"orderIds={id:D}"));
        return await http.GetFromJsonAsync<IReadOnlyList<NotificationDto>>(
                   $"/Notifications/ByOrders?{query}", ct)
               ?? Array.Empty<NotificationDto>();
    }
}

public sealed class ProcessingSystemClient(HttpClient http)
{
    public async Task<IReadOnlyList<ProcessingCameraFile>> ListCamerasAsync(CancellationToken ct)
    {
        return await http.GetFromJsonAsync<IReadOnlyList<ProcessingCameraFile>>("/Cameras", ct)
               ?? Array.Empty<ProcessingCameraFile>();
    }
}

public sealed class UserServiceClient(HttpClient http)
{
    public async Task<ValidationResult> ValidateAccessAsync(Guid userId, Guid cameraId, CancellationToken ct)
    {
        var response = await http.PostAsJsonAsync(
            "/UserAccess/Validate",
            new { UserId = userId, CameraId = cameraId },
            ct);

        if (!response.IsSuccessStatusCode)
        {
            return new ValidationResult(false, $"Access check failed: {(int)response.StatusCode}");
        }

        var body = await response.Content.ReadFromJsonAsync<UserAccessResponse>(cancellationToken: ct);
        return new ValidationResult(body?.IsAllowed ?? false, body?.DenyReason);
    }

    public Task<AuthResult> LoginAsync(string login, string password, CancellationToken ct) =>
        AuthAsync("/Auth/Login", login, password, ct);

    public Task<AuthResult> RegisterAsync(string login, string password, CancellationToken ct) =>
        AuthAsync("/Auth/Register", login, password, ct);

    private async Task<AuthResult> AuthAsync(string path, string login, string password, CancellationToken ct)
    {
        var response = await http.PostAsJsonAsync(path, new { Login = login, Password = password }, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<AuthenticatedUser>(cancellationToken: ct);
            return body is null
                ? new AuthResult(null, "Empty response from auth service.")
                : new AuthResult(body, null);
        }

        var errorBody = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: ct);
        return new AuthResult(null, errorBody?.Error ?? $"Auth failed: {(int)response.StatusCode}");
    }

    private sealed record UserAccessResponse(bool IsAllowed, string? DenyReason);
    private sealed record ErrorResponse(string? Error);
}

public sealed record AuthenticatedUser(Guid UserId, string Login);

public sealed record AuthResult(AuthenticatedUser? User, string? Error);

public sealed record VideoArchiveCameraDto(Guid Id, string Name, bool IsActive);

public sealed class VideoArchiveServiceClient(HttpClient http)
{
    public async Task<IReadOnlyList<VideoArchiveCameraDto>> ListCamerasAsync(CancellationToken ct)
    {
        return await http.GetFromJsonAsync<IReadOnlyList<VideoArchiveCameraDto>>("/Cameras/List", ct)
               ?? Array.Empty<VideoArchiveCameraDto>();
    }

    public async Task<ValidationResult> ValidateAvailabilityAsync(
        Guid cameraId,
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        CancellationToken ct)
    {
        var response = await http.PostAsJsonAsync(
            "/ArchiveValidation/Validate",
            new { CameraId = cameraId, FromUtc = fromUtc, ToUtc = toUtc },
            ct);

        if (!response.IsSuccessStatusCode)
        {
            return new ValidationResult(false, $"Availability check failed: {(int)response.StatusCode}");
        }

        var body = await response.Content.ReadFromJsonAsync<ArchiveAvailabilityResponse>(cancellationToken: ct);
        return new ValidationResult(body?.IsAvailable ?? false, body?.DenyReason);
    }

    private sealed record ArchiveAvailabilityResponse(bool IsAvailable, string? DenyReason);
}

public sealed class OrderServiceClient(HttpClient http)
{
    public HttpClient Http => http;

    public async Task<IReadOnlyList<OrderDto>> ListByUserAsync(Guid userId, CancellationToken ct)
    {
        return await http.GetFromJsonAsync<IReadOnlyList<OrderDto>>($"/Orders/List?userId={userId}", ct)
               ?? Array.Empty<OrderDto>();
    }

    public async Task<OrderDto?> GetByIdAsync(Guid orderId, CancellationToken ct)
    {
        var response = await http.GetAsync($"/Orders/GetById/{orderId}", ct);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<OrderDto>(cancellationToken: ct);
    }

    public async Task<(HttpResponseMessage response, OrderDto? order)> CreateAsync(
        Guid userId,
        Guid cameraId,
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        CancellationToken ct)
    {
        var response = await http.PostAsJsonAsync(
            "/Orders/Create",
            new { UserId = userId, CameraId = cameraId, FromUtc = fromUtc, ToUtc = toUtc },
            ct);

        if (!response.IsSuccessStatusCode)
        {
            return (response, null);
        }

        var order = await response.Content.ReadFromJsonAsync<OrderDto>(cancellationToken: ct);
        return (response, order);
    }
}
