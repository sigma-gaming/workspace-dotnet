using Core.Contracts.Data;

namespace Core.Contracts.Extensions;

public static class DomainExtensions
{
    public static Domain ToData(this Contracts.Entities.DomainEntity entity)
    {
        return new Domain
        {
            Host = entity.Host,
            CreatedAt = entity.CreatedAt,
            App = entity.App
        };
    }
} 