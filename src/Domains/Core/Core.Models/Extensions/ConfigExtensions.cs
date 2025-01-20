using Core.Domain.Entities;
using Core.Models.Data;
using Core.Models.Entities;

namespace Core.Domain.Entities;

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