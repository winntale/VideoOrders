using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using UserServiceClient.Abstractions;
using UserServiceClient.Abstractions.Clients;
using UserServiceClient.Abstractions.Models;
using UserServiceClient.Abstractions.Options;

namespace UserServiceClient.Adapter.Clients;

internal sealed class UserServiceApiClient(
    IHttpClientFactory httpClientFactory,
    IOptions<UserServiceClientOptions> options)
    : IUserServiceApiClient
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(UserServiceApiClient)); 
    private readonly string _baseUrl = options.Value.BaseUrl;
    
    public async Task<Result<ValidateAccessResultClientModel?>> ValidateUserAccessAsync(
        ValidateAccessClientModel model,
        CancellationToken cancellationToken)
    {
        
        using var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl.TrimEnd('/')}/UserAccess/Validate",
            model,
            cancellationToken);
    
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Error.NotFound($"User with id '{model.UserId}' was not found.");
        }

        if (!response.IsSuccessStatusCode)
        {
            return new ValidateAccessResultClientModel
            {
                IsAllowed = false,
                DenyReason = $"UserService returned status code {(int)response.StatusCode}."
            };
        }

        var responseModel =
            await response.Content.ReadFromJsonAsync<ValidateAccessResultClientModel>(cancellationToken);

        return responseModel;
    }
}