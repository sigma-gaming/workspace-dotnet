using Core.Domain.Data;
using Core.Domain.Entities;

namespace Core.Domain.Entities;

public static class PromocodeUsageExtensions
{
    public static PromocodeUsageData ToData(this PromocodeUsage entity)
    {
        return new PromocodeUsageData
        {
            CreatedAt = entity.CreatedAt,
            PromocodeId = entity.PromocodeId,
            UserId = entity.UserId,
            ExpiresAt = entity.ExpiresAt,
            Status = entity.Status
        };
    }
} 