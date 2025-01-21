namespace Core.PubSub.Configuration;

public class RedisPubSubOptions
{
    public string Host { get; set; } = null!;
    public int Port { get; set; } = 6379;
    public string Password { get; set; } = null!;
    public string Version { get; set; } = "v1";
}