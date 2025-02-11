using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.User;

public class UserStatsService(CoreDbContext db, UserStatsMapper mapper)
{
    private async Task<UserStatsEntity?> QueryStatsAsync(Guid userId)
    {
        return await db.UserStats
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<UserStats> GetStatsAsync(Guid userId)
    {
        var stats = await QueryStatsAsync(userId);
        if (stats is null) throw new Exception("User stats not found");
        return mapper.ToData(stats);
    }
}