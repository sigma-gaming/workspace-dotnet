using Core.Domain.Data;
using Core.Domain.Entities;

namespace Core.Domain.Entities;

public static class TransactionExtensions
{
    public static TransactionData ToData(this Transaction entity)
    {
        return new TransactionData
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