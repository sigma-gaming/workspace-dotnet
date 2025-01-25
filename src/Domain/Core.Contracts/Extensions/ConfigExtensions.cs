using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ConfigExtensions
{
    public static Config ToData(this ConfigEntity entity)
    {
        return new Config
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            MaintenanceEnabled = entity.MaintenanceEnabled,
            BackgroundJobsEnabled = entity.BackgroundJobsEnabled
        };
    }
} 