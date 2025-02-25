namespace LetsAuth.API.Configuration;

public class ApiOptions
{
    public string ClientUrl { get; set; } = null!;
    public string PublicUrl { get; set; } = null!;
    public bool EnableOpenApi { get; set; } = false;
    public string? RateLimitBypassToken { get; set; }
}