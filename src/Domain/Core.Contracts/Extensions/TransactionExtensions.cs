using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class TransactionExtensions
{
    public static Transaction ToData(this TransactionEntity entity)
    {
        return new Transaction
        {
            CreatedAt = entity.CreatedAt,
            Amount = entity.Amount,
            UserId = entity.UserId,
            GameRecordId = entity.GameRecordId,
            Id = entity.Id,
            Type = entity.Type,
            Game = entity.Game
        };
    }
}