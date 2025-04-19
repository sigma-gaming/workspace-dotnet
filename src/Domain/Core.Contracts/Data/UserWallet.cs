using Core.Contracts.Enums;

namespace Core.Contracts.Data;

public sealed record UserWallet
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required PaymentProvider Provider { get; init; }
    public required Currency Currency { get; init; }
    public required string WalletAddress { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required bool IsActive { get; init; }
}
