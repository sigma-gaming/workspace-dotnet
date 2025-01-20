using Core.Domain.Data;
using Core.Domain.Entities;

namespace Core.Domain.Entities;

public static class BudgetExtensions
{
    public static BudgetData ToData(this Budget entity)
    {
        return new BudgetData
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            LastSyncAt = entity.LastSyncAt,
            Available = entity.Available
        };
    }
} 