using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ProfileExtensions
{
    public static ProfileData ToData(this Profile entity)
    {
        return new ProfileData
        {
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Name = entity.Name,
            Username = entity.Username,
            UserId = entity.UserId,
            HasCustomName = entity.HasCustomName,
            Image = entity.Image,
            Id = entity.Id,
            UsedProvider = entity.UsedProvider
        };
    }
} 