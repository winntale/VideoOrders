using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using UserService.Abstractions.Clients;
using UserService.Abstractions.Models;
using UserService.Abstractions.Options;

namespace UserServiceClient.Adapter.Clients;

internal sealed class UserServiceApiClient(
    IHttpClientFactory httpClientFactory,
    IOptions<UserServiceClientOptions> options)
    : IUserServiceApiClient
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(UserServiceClientOptions.HttpClientName); 
    private readonly string _baseUrl = options.Value.BaseUrl;
    
    public async Task<ValidateAccessResultClientModel?> ValidateUserAccessAsync(
        ValidateAccessClientModel model,
        CancellationToken cancellationToken)
    {
        
        using var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl.TrimEnd('/')}/api/users/access",
            model,
            cancellationToken);
    
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new ValidateAccessResultClientModel
            {
                UserId = model.UserId,
                CameraId = model.CameraId,
                IsAllowed = false,
                DenyReason = $"User with id '{model.UserId}' was not found.",
                UserNotFound = true
            };
        }

        if (!response.IsSuccessStatusCode)
        {
            return new ValidateAccessResultClientModel
            {
                UserId = model.UserId,
                CameraId = model.CameraId,
                IsAllowed = false,
                DenyReason = $"UserService returned status code {(int)response.StatusCode}.",
                UserNotFound = false
            };
        }

        var responseModel =
            await response.Content.ReadFromJsonAsync<ValidateAccessResultClientModel>(cancellationToken);

        return responseModel;
    }
}