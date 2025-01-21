using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class BudgetExtensions
{
    public static Budget ToData(this BudgetEntity entity)
    {
        return new Budget
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            LastSyncAt = entity.LastSyncAt,
            Available = entity.Available
        };
    }
} 