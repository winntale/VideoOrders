namespace Gateway.Options;

public sealed record CameraInputOptions
{
    public string RootPath { get; init; } = "/app/input";
}
