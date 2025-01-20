using Core.Domain.Data;
using Core.Domain.Entities;

namespace Core.Domain.Entities;

public static class DomainExtensions
{
    public static DomainData ToData(this Core.Domain.Entities.Domain entity)
    {
        return new DomainData
        {
            Host = entity.Host,
            CreatedAt = entity.CreatedAt,
            App = entity.App
        };
    }
} 