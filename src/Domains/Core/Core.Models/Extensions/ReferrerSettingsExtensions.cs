using Core.Domain.Entities;
using Core.Models.Data;
using Core.Models.Entities;

namespace Core.Domain.Entities;

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