using Core.Domain.Data;
using Core.Domain.Entities;

namespace Core.Domain.Entities;

public static class GameRecordExtensions
{
    public static GameRecordData ToData(this GameRecord entity)
    {
        return new GameRecordData
        {
            Game = entity.Game,
            Outcome = entity.Outcome,
            CreatedAt = entity.CreatedAt,
            Snapshot = entity.Snapshot,
            Bet = entity.Bet,
            Multiplier = entity.Multiplier,
            Payout = entity.Payout,
            PreviewUserName = entity.PreviewUserName,
            UserId = entity.UserId,
            TransactionId = entity.TransactionId,
            Id = entity.Id
        };
    }
} 