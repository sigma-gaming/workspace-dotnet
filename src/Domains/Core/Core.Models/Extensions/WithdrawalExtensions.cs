using Core.Domain.Entities;
using Core.Models.Data;
using Core.Models.Entities;

namespace Core.Domain.Entities;

public static class WithdrawalExtensions
{
    public static WithdrawalData ToData(this Withdrawal entity)
    {
        return new WithdrawalData
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