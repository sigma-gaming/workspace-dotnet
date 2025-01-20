using Core.Domain.Entities;
using Core.Models.Data;
using Core.Models.Entities;

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