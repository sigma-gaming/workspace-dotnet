using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ReferrerWithdrawalExtensions
{
    public static ReferrerWithdrawalData ToData(this ReferrerWithdrawal entity)
    {
        return new ReferrerWithdrawalData
        {
            CreatedAt = entity.CreatedAt,
            Amount = entity.Amount,
            ReferrerId = entity.ReferrerId,
            Id = entity.Id
        };
    }
} 