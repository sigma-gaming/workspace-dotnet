namespace Core.Services.Gaming.Runners;

public record BaseGamePayload
{
    public required long Bet { get; init; }
}