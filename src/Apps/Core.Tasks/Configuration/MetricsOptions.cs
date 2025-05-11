namespace Core.Tasks.Configuration;

/// <summary>
///     Options for metrics configuration
/// </summary>
public class MetricsOptions
{
    public bool Enabled { get; set; } = false;

    /// <summary>
    ///     The URL of the Prometheus Pushgateway
    /// </summary>
    public string? PushgatewayUrl { get; set; }

    /// <summary>
    ///     The namespace for metrics
    /// </summary>
    public string? Namespace { get; set; }
}