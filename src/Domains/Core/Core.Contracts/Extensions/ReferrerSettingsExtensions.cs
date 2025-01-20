using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ReferrerSettingsExtensions
{
    public static ReferrerSettingsData ToData(this ReferrerSettings entity)
    {
        return new ReferrerSettingsData
        {
            ReferrerId = entity.ReferrerId,
            RevShare = entity.RevShare
        };
    }
} 