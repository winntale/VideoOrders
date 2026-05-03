using Dal.Clients;
using Dal.ElasticModels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dal.HostedServices;

public sealed class ElasticsearchIndexInitializer(
    AuditElasticsearchClient elasticClient,
    ILogger<ElasticsearchIndexInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var existsResponse = await elasticClient.Client.Indices.ExistsAsync(
            elasticClient.IndexName,
            cancellationToken);

        if (existsResponse.Exists)
        {
            logger.LogInformation("Index {IndexName} already exists", elasticClient.IndexName);
            return;
        }

        try
        {
            var createResponse = await elasticClient.Client.Indices.CreateAsync(
                elasticClient.IndexName,
                c => c.Mappings(m => m.Properties<AuditEntryDocument>(p => p
                    .Keyword(x => x.Id)
                    .Keyword(x => x.OrderId)
                    .Keyword(x => x.UserId)
                    .Keyword(x => x.CameraId)
                    .Keyword(x => x.ServiceName)
                    .Keyword(x => x.EventType)
                    .Keyword(x => x.EventAction)
                    .Keyword(x => x.EventOutcome)
                    .Keyword(x => x.StatusFrom)
                    .Keyword(x => x.StatusTo)
                    .Keyword(x => x.FailureReason)
                    .Text(x => x.PayloadJson)
                    .Date(x => x.OccurredAtUtc)
                )),
                cancellationToken);

            if (!createResponse.IsValidResponse)
            {
                logger.LogError("Failed to create index {IndexName}. Debug: {Debug}",
                    elasticClient.IndexName,
                    createResponse.DebugInformation);
                return;
            }

            logger.LogInformation("Index {IndexName} created", elasticClient.IndexName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create index {IndexName}", elasticClient.IndexName);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}