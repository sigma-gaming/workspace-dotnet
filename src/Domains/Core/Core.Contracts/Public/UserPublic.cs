using Core.Contracts.Enums;

namespace Core.Contracts.Public;

public sealed record UserPublic
{
    public required Guid Id { get; init; }
    public int? ProfileId { get; init; }
    public required UserRole[] Roles { get; init; }
}