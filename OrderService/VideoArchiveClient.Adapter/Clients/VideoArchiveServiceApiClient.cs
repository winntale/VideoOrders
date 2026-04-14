using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using VideoArchiveClient.Abstractions;
using VideoArchiveClient.Abstractions.Clients;
using VideoArchiveClient.Abstractions.Models;
using VideoArchiveClient.Abstractions.Options;

namespace VideoArchiveClient.Adapter.Clients;

internal sealed class VideoArchiveServiceApiClient(
    IHttpClientFactory httpClientFactory,
    IOptions<VideoArchiveServiceClientOptions> options)
    : IVideoArchiveServiceApiClient
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(VideoArchiveServiceApiClient));

    private readonly string _baseUrl = options.Value.BaseUrl;

    public async Task<Result<ValidateArchiveAvailabilityResultClientModel?>> ValidateArchiveAvailabilityAsync(
        ValidateArchiveAvailabilityClientModel clientModel,
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl.TrimEnd('/')}/ArchiveValidation/Validate",
            clientModel,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new ValidateArchiveAvailabilityResultClientModel
            {
                IsAvailable = false,
                DenyReason = $"Camera '{clientModel.CameraId}' was not found."
            };
        }

        if (!response.IsSuccessStatusCode)
        {
            return new ValidateArchiveAvailabilityResultClientModel
            {
                IsAvailable = false,
                DenyReason = $"VideoArchiveService returned status code '{(int)response.StatusCode}'"
            };
        }

        var responseModel =
            await response.Content.ReadFromJsonAsync<ValidateArchiveAvailabilityResultClientModel>(cancellationToken);

        return responseModel;
    }
}