namespace Core.Nats.Configuration;

public class NatsOptions
{
    public string Url { get; set; } = null!;
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = null!;
}