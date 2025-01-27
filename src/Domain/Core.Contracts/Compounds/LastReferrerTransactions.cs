using Core.Contracts.Data;

namespace Core.Contracts.Compounds;

public record LastReferrerTransactions
{
    public required ReferrerTransaction[] Transactions { get; init; }
    public required long TotalAmount { get; init; }
}