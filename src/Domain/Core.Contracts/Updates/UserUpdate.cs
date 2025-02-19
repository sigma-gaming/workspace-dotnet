using System.ComponentModel.DataAnnotations;

namespace Core.Contracts.Updates;

public sealed record UserUpdate<T>
{
    public required Guid UserId { get; init; }

    [Required] public long Time { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public required UpdateMode Mode { get; init; }
    [Required] public required T Data { get; init; }
}