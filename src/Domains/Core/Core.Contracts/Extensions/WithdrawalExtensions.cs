using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class WithdrawalExtensions
{
    public static Withdrawal ToData(this WithdrawalEntity entity)
    {
        return new Withdrawal
        {
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            GemAmount = entity.GemAmount,
            CurrencyAmount = entity.CurrencyAmount,
            UserId = entity.UserId,
            TransactionId = entity.TransactionId,
            Id = entity.Id,
            ProviderTransactionId = entity.ProviderTransactionId,
            ProviderAmount = entity.ProviderAmount,
            Status = entity.Status,
            Currency = entity.Currency,
            Method = entity.Method,
            Provider = entity.Provider
        };
    }
} 