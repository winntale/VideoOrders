using System.Net.Http.Json;
using VideoOrdersPortal.Models;

namespace VideoOrdersPortal.Clients;

public sealed class ProcessingSystemClient(HttpClient http)
{
    public async Task<IReadOnlyList<CameraDto>> ListCamerasAsync(CancellationToken ct)
    {
        return await http.GetFromJsonAsync<IReadOnlyList<CameraDto>>("/Cameras", ct)
               ?? Array.Empty<CameraDto>();
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

    private sealed record UserAccessResponse(bool IsAllowed, string? DenyReason);
}

public sealed class VideoArchiveServiceClient(HttpClient http)
{
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
