using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class DepositExtensions
{
    public static Deposit ToData(this DepositEntity entity)
    {
        return new Deposit
        {
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            GemAmount = entity.GemAmount,
            CurrencyAmount = entity.CurrencyAmount,
            Payload = entity.Payload,
            UserId = entity.UserId,
            TransactionId = entity.TransactionId,
            Id = entity.Id,
            ProviderTransactionId = entity.ProviderTransactionId,
            ProviderAmount = entity.ProviderAmount,
            Status = entity.Status,
            Currency = entity.Currency,
            Method = entity.Method,
            Provider = entity.Provider,
            Type = entity.Type
        };
    }
} 