using Core.Contracts.Data;
using Core.Contracts.Public;

namespace Core.Contracts.Compounds;

public sealed record ProfileDetailed
{
    public required UserPublic User { get; init; }
    public required Profile Profile { get; init; }
    public required Account[] Accounts { get; init; }
}