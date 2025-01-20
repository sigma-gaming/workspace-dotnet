using Core.Contracts.Data;

namespace Core.Contracts.Extensions;

public static class DomainExtensions
{
    public static DomainData ToData(this Contracts.Entities.Domain entity)
    {
        return new DomainData
        {
            Host = entity.Host,
            CreatedAt = entity.CreatedAt,
            App = entity.App
        };
    }
} 