using Core.Domain.Data;
using Core.Domain.Entities;

namespace Core.Domain.Entities;

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