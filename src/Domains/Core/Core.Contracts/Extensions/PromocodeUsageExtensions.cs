using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

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