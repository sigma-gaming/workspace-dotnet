using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ReferrerSettingsExtensions
{
    public static ReferrerSettings ToData(this ReferrerSettingsEntity entity)
    {
        return new ReferrerSettings
        {
            ReferrerId = entity.ReferrerId,
            RevShare = entity.RevShare
        };
    }
} 