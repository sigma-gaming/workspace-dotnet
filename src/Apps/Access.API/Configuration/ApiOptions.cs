namespace Access.API.Configuration;

public class ApiOptions
{
    public string PublicUrl { get; set; } = null!;
    public bool EnableOpenApi { get; set; } = false;
    public string? RateLimitBypassToken { get; set; }
}