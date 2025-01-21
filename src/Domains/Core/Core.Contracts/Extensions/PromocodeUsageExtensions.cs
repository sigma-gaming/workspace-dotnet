using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class PromocodeUsageExtensions
{
    public static PromocodeUsage ToData(this PromocodeUsageEntity entity)
    {
        return new PromocodeUsage
        {
            CreatedAt = entity.CreatedAt,
            PromocodeId = entity.PromocodeId,
            UserId = entity.UserId,
            ExpiresAt = entity.ExpiresAt,
            Status = entity.Status
        };
    }
} 