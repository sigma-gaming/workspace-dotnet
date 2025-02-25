namespace Common.Api.Configuration;

public record HttpOptions
{
    public int PublicPort { get; set; } = 8080;
    public int InternalPort { get; set; } = 9090;
}