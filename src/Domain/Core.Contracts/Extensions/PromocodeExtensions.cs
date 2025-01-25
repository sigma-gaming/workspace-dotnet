using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class PromocodeExtensions
{
    public static Promocode ToData(this PromocodeEntity entity)
    {
        return new Promocode
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            ExpiresAt = entity.ExpiresAt,
            Code = entity.Code,
            WageringMultiplier = entity.WageringMultiplier,
            Usages = entity.Usages,
            MaxUsages = entity.MaxUsages,
            IsActive = entity.IsActive,
            Bonus = entity.Bonus,
            UserId = entity.UserId,
            Campaign = entity.Campaign,
            CreatedBy = entity.CreatedBy,
            BonusType = entity.BonusType
        };
    }
} 