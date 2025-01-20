using Core.Domain.Data;
using Core.Domain.Entities;

namespace Core.Domain.Entities;

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