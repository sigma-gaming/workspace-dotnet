using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ConfigExtensions
{
    public static ConfigData ToData(this Config entity)
    {
        return new ConfigData
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            MaintenanceEnabled = entity.MaintenanceEnabled,
            BackgroundJobsEnabled = entity.BackgroundJobsEnabled
        };
    }
} 