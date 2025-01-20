using Core.Domain.Entities;
using Core.Models.Data;
using Core.Models.Entities;

namespace Core.Domain.Entities;

public static class PromocodeExtensions
{
    public static PromocodeData ToData(this Promocode entity)
    {
        return new PromocodeData
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