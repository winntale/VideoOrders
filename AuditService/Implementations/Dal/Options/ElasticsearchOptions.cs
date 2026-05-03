namespace Dal.Options;

public sealed record ElasticsearchOptions
{
    public string Url { get; init; } = null!;
    public string IndexName { get; init; } = null!;
}