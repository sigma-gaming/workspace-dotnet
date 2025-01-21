using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ReferrerWithdrawalExtensions
{
    public static ReferrerWithdrawal ToData(this ReferrerWithdrawalEntity entity)
    {
        return new ReferrerWithdrawal
        {
            CreatedAt = entity.CreatedAt,
            Amount = entity.Amount,
            ReferrerId = entity.ReferrerId,
            Id = entity.Id
        };
    }
} 