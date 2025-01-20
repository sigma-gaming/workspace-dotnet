using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

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