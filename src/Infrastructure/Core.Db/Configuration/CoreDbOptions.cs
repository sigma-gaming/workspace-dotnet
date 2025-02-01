namespace Core.Db.Configuration;

public class CoreDbOptions
{
    public string Host { get; set; } = null!;
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int MaxConnections { get; set; } = 100;
}