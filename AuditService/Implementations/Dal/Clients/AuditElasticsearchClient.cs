using Dal.Options;
using Elastic.Clients.Elasticsearch;

namespace Dal.Clients;

public sealed class AuditElasticsearchClient
{
    public AuditElasticsearchClient(ElasticsearchOptions options)
    {
        var settings = new ElasticsearchClientSettings(new Uri(options.Url))
            .DefaultIndex(options.IndexName);

        Client = new ElasticsearchClient(settings);
        IndexName = options.IndexName;
    }
    
    public ElasticsearchClient Client { get; }
    public string IndexName { get; }
}