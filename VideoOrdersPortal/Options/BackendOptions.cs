namespace VideoOrdersPortal.Options;

public sealed class BackendOptions
{
    public string OrderServiceUrl { get; set; } = null!;
    public string UserServiceUrl { get; set; } = null!;
    public string VideoArchiveServiceUrl { get; set; } = null!;
    public string ProcessingSystemUrl { get; set; } = null!;
    public string NotificationServiceUrl { get; set; } = null!;
}
