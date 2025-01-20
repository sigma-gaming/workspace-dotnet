using Core.Domain.Entities;
using Core.Models.Data;

namespace Core.Domain.Entities;

public static class DomainExtensions
{
    public static DomainData ToData(this Models.Entities.Domain entity)
    {
        return new DomainData
        {
            Host = entity.Host,
            CreatedAt = entity.CreatedAt,
            App = entity.App
        };
    }
} 