using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

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