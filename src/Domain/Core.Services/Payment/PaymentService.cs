using Common.Db.Extensions;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Contracts.Public;
using Core.Contracts.Updates;
using Core.Db.Context;
using Core.Services.Caching;
using Core.Services.Finance;
using Core.Services.Payment.Configuration;
using Core.Services.Payment.Exceptions;
using Core.Services.Payment.Interfaces;
using Core.Services.Payment.Models;
using Core.Services.Payment.Providers;
using Core.Services.PubSub;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services.Payment;

/// <summary>
///     Service for processing payments, deposits and withdrawals
/// </summary>
public class PaymentService(
    CoreDbContext db,
    BalanceService balanceService,
    PaymentProviderFactory providerFactory,
    WalletService walletService,
    CoreCacheRegistry cache,
    CorePubSubRegistry pubSub,
    BalanceMapper balanceMapper,
    DepositMapper depositMapper,
    WithdrawalMapper withdrawalMapper,
    IDepositConfigProvider depositConfigProvider,
    IWithdrawalConfigProvider withdrawalConfigProvider,
    ILogger<PaymentService>? logger = null) : IPaymentService
{
    private List<DepositMethodConfig> DepositConfigs { get; } = depositConfigProvider.GetConfigs();
    private List<WithdrawalMethodConfig> WithdrawalConfigs { get; } = withdrawalConfigProvider.GetConfigs();

    public virtual List<DepositMethodConfig> GetDepositConfigs()
    {
        return DepositConfigs;
    }

    public virtual List<WithdrawalMethodConfig> GetWithdrawalConfigs()
    {
        // Filter out providers that don't support withdrawals
        return WithdrawalConfigs.Where(c => SupportsWithdrawal(c.Provider)).ToList();
    }

    public virtual async Task<DepositOutput> CreateDepositAsync(DepositInput input)
    {
        try
        {
            // Найти подходящий конфиг метода депозита
            var config = DepositConfigs.FirstOrDefault(c =>
                c.Method == input.Method &&
                c.Currency == input.Currency &&
                c.Provider == input.Provider);

            if (config is null)
                throw ExceptionFactory.UnsupportedDepositMethod(
                    input.Provider,
                    input.Method,
                    input.Currency);

            if (input.GemAmount < config.MinAmount || input.GemAmount > config.MaxAmount)
                throw ExceptionFactory.InvalidAmount(
                    input.GemAmount,
                    config.MinAmount,
                    config.MaxAmount,
                    input.Currency);

            // Получить провайдера
            var provider = providerFactory.GetDepositProvider(input.Provider);
            if (provider is null)
                throw ExceptionFactory.ProviderError(
                    input.Provider,
                    "Deposit provider not available");

            // Создать депозит у провайдера
            var response = await provider.CreateDepositAsync(input);

            return await db.ExecuteInTransactionAsync(async () =>
            {
                var deposit = new DepositEntity
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = response.CreatedAt,
                    UpdatedAt = response.UpdatedAt,
                    GemAmount = input.GemAmount,
                    CurrencyAmount = decimal.TryParse(response.ProviderAmount, out var ca) ? ca : 0,
                    FlowDetails = response.FlowDetails,
                    UserId = input.UserId,
                    ProviderTransactionId = response.ProviderTransactionId,
                    ProviderAmount = response.ProviderAmount,
                    Status = response.Status,
                    Currency = input.Currency,
                    Method = input.Method,
                    Provider = input.Provider,
                    Flow = response.Flow,
                };

                db.Deposits.Add(deposit);
                await db.SaveChangesAsync();

                return new DepositOutput
                {
                    Outcome = PaymentOutcome.Success,
                    Deposit = depositMapper.ToData(deposit),
                };
            });
        }
        catch (PaymentException ex)
        {
            logger?.LogWarning(ex, "Payment error during deposit creation");

            if (ex is UnsupportedPaymentMethodException || ex is PaymentProviderException)
                return new DepositOutput
                {
                    Outcome = PaymentOutcome.UnsupportedMethod,
                    Method = input.Method,
                    Currency = input.Currency,
                    Provider = input.Provider,
                    Error = ex.Message,
                };

            if (ex is InvalidAmountException invalidAmount)
                return new DepositOutput
                {
                    Outcome = PaymentOutcome.InvalidAmount,
                    MinAmount = invalidAmount.MinAmount,
                    MaxAmount = invalidAmount.MaxAmount,
                    Currency = input.Currency,
                    Method = input.Method,
                    Provider = input.Provider,
                    Error = ex.Message,
                };

            return new DepositOutput
            {
                Outcome = PaymentOutcome.Failed,
                Error = ex.Message,
            };
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unexpected error during deposit creation");
            return new DepositOutput
            {
                Outcome = PaymentOutcome.Failed,
                Error = "Internal server error",
            };
        }
    }

    public async Task<bool> ConfirmDepositAsync(Guid depositId)
    {
        try
        {
            var deposit = await db.Deposits.FirstOrDefaultAsync(d => d.Id == depositId);
            if (deposit is null) throw ExceptionFactory.TransactionNotFound(depositId, "Deposit");

            if (deposit.Status != PaymentStatus.Pending)
                throw ExceptionFactory.InvalidStatusTransition("deposit", deposit.Status, "confirm");

            return await db.ExecuteInTransactionAsync(async () =>
            {
                deposit.Status = PaymentStatus.Completed;
                deposit.UpdatedAt = DateTime.UtcNow;

                var transaction = new TransactionEntity
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Amount = deposit.GemAmount,
                    UserId = deposit.UserId,
                    Type = TransactionType.Deposit,
                };
                db.Transactions.Add(transaction);
                deposit.TransactionId = transaction.Id;
                deposit.Transaction = transaction;

                var balance = await balanceService.GetBalanceForUpdateAsync(deposit.UserId);
                var updatedBalance = await balanceService.UpdateBalanceAsync(balance, transaction);

                await db.SaveChangesAsync();

                // Update cache and publish balance update
                await cache.UserBalance.TrySetAsync(deposit.UserId, balanceMapper.ToData(updatedBalance));
                await pubSub.BalanceUpdated.TryPublishAsync(new UserUpdate<BalancePublic>
                {
                    UserId = deposit.UserId,
                    Mode = UpdateMode.Optimized,
                    Data = balanceMapper.ToPublic(updatedBalance),
                });

                logger?.LogInformation("Deposit {DepositId} confirmed successfully", depositId);
                return true;
            });
        }
        catch (TransactionNotFoundException)
        {
            return false;
        }
        catch (PaymentException ex)
        {
            logger?.LogWarning(ex, "Error confirming deposit {DepositId}", depositId);
            return false;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unexpected error confirming deposit {DepositId}", depositId);
            return false;
        }
    }

    public async Task<bool> CancelDepositAsync(Guid depositId)
    {
        try
        {
            var deposit = await db.Deposits.FirstOrDefaultAsync(d => d.Id == depositId);
            if (deposit is null) throw ExceptionFactory.TransactionNotFound(depositId, "Deposit");

            if (deposit.Status != PaymentStatus.Pending)
                throw ExceptionFactory.InvalidStatusTransition("deposit", deposit.Status, "cancel");

            return await db.ExecuteInTransactionAsync(async () =>
            {
                deposit.Status = PaymentStatus.Cancelled;
                deposit.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();

                logger?.LogInformation("Deposit {DepositId} cancelled successfully", depositId);
                return true;
            });
        }
        catch (TransactionNotFoundException)
        {
            return false;
        }
        catch (PaymentException ex)
        {
            logger?.LogWarning(ex, "Error cancelling deposit {DepositId}", depositId);
            return false;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unexpected error cancelling deposit {DepositId}", depositId);
            return false;
        }
    }

    public virtual async Task<WithdrawalOutput> CreateWithdrawalAsync(WithdrawalInput input)
    {
        try
        {
            // Find matching withdrawal method config
            var config = WithdrawalConfigs.FirstOrDefault(c =>
                c.Method == input.Method &&
                c.Currency == input.Currency &&
                c.Provider == input.Provider);

            if (config is null)
                throw ExceptionFactory.UnsupportedWithdrawalMethod(
                    input.Provider,
                    input.Method,
                    input.Currency);

            if (input.GemAmount < config.MinAmount || input.GemAmount > config.MaxAmount)
                throw ExceptionFactory.InvalidAmount(
                    input.GemAmount,
                    config.MinAmount,
                    config.MaxAmount,
                    input.Currency);

            // Check if provider supports withdrawals
            if (!SupportsWithdrawal(input.Provider))
                throw ExceptionFactory.ProviderError(
                    input.Provider,
                    "Provider does not support withdrawals");

            // Get provider
            var provider = GetWithdrawalProvider(input.Provider);
            if (provider is null)
                throw ExceptionFactory.ProviderError(
                    input.Provider,
                    "Withdrawal provider not available");

            return await db.ExecuteInTransactionAsync(async () =>
            {
                // Check user balance
                var balance = await balanceService.GetBalanceForUpdateAsync(input.UserId);
                if (balance.Available < input.GemAmount)
                    return new WithdrawalOutput
                    {
                        Outcome = PaymentOutcome.InsufficientFunds,
                        Available = balance.Available,
                    };

                // Create withdrawal with provider
                var response = await provider.CreateWithdrawalAsync(input);

                // Create withdrawal entity
                var withdrawal = new WithdrawalEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = input.UserId,
                    GemAmount = input.GemAmount,
                    CurrencyAmount = decimal.TryParse(response.ProviderAmount, out var ca) ? ca : 0,
                    Currency = input.Currency,
                    Provider = input.Provider,
                    Method = input.Method,
                    Status = PaymentStatus.Pending,
                    ProviderTransactionId = response.ProviderTransactionId,
                    ProviderAmount = response.ProviderAmount,
                    CreatedAt = response.CreatedAt,
                    UpdatedAt = response.UpdatedAt,
                    // PaymentDetails is stored in external metadata if needed
                };

                db.Withdrawals.Add(withdrawal);

                // Create transaction to decrease user balance
                var transaction = new TransactionEntity
                {
                    Id = Guid.NewGuid(),
                    Amount = -input.GemAmount,
                    UserId = input.UserId,
                    CreatedAt = DateTime.UtcNow,
                    Type = TransactionType.Withdrawal,
                };

                db.Transactions.Add(transaction);
                withdrawal.TransactionId = transaction.Id;
                withdrawal.Transaction = transaction;

                // Update user balance
                var updatedBalance = await balanceService.UpdateBalanceAsync(balance, transaction);

                await db.SaveChangesAsync();

                // Update cache and publish balance update
                await cache.UserBalance.TrySetAsync(withdrawal.UserId, balanceMapper.ToData(updatedBalance));
                await pubSub.BalanceUpdated.TryPublishAsync(new UserUpdate<BalancePublic>
                {
                    UserId = withdrawal.UserId,
                    Mode = UpdateMode.Optimized,
                    Data = balanceMapper.ToPublic(updatedBalance),
                });

                logger?.LogInformation("Withdrawal {WithdrawalId} created successfully", withdrawal.Id);

                return new WithdrawalOutput
                {
                    Outcome = PaymentOutcome.Success,
                    Withdrawal = withdrawalMapper.ToData(withdrawal),
                    UpdatedBalance = updatedBalance.Available,
                };
            });
        }
        catch (PaymentException ex)
        {
            logger?.LogWarning(ex, "Payment error during withdrawal creation");

            if (ex is UnsupportedPaymentMethodException || ex is PaymentProviderException)
                return new WithdrawalOutput
                {
                    Outcome = PaymentOutcome.UnsupportedMethod,
                    Method = input.Method,
                    Currency = input.Currency,
                    Provider = input.Provider,
                    Error = ex.Message,
                };

            if (ex is InvalidAmountException invalidAmount)
                return new WithdrawalOutput
                {
                    Outcome = PaymentOutcome.InvalidAmount,
                    MinAmount = invalidAmount.MinAmount,
                    MaxAmount = invalidAmount.MaxAmount,
                    Currency = input.Currency,
                    Method = input.Method,
                    Provider = input.Provider,
                    Error = ex.Message,
                };

            return new WithdrawalOutput
            {
                Outcome = PaymentOutcome.Failed,
                Error = ex.Message,
            };
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unexpected error during withdrawal creation");
            return new WithdrawalOutput
            {
                Outcome = PaymentOutcome.Failed,
                Error = "Internal server error",
            };
        }
    }

    public async Task<bool> ApproveWithdrawalAsync(Guid withdrawalId, Guid adminId)
    {
        try
        {
            var withdrawal = await db.Withdrawals.FirstOrDefaultAsync(w => w.Id == withdrawalId);
            if (withdrawal is null) throw ExceptionFactory.TransactionNotFound(withdrawalId, "Withdrawal");

            if (withdrawal.Status != PaymentStatus.Pending)
                throw ExceptionFactory.InvalidStatusTransition("withdrawal", withdrawal.Status, "approve");

            return await db.ExecuteInTransactionAsync(async () =>
            {
                withdrawal.Status = PaymentStatus.Processing;
                withdrawal.UpdatedAt = DateTime.UtcNow;
                // Store admin ID in a comment or metadata if needed

                await db.SaveChangesAsync();

                logger?.LogInformation("Withdrawal {WithdrawalId} approved by admin {AdminId}", withdrawalId, adminId);
                return true;
            });
        }
        catch (TransactionNotFoundException)
        {
            return false;
        }
        catch (PaymentException ex)
        {
            logger?.LogWarning(ex, "Error approving withdrawal {WithdrawalId}", withdrawalId);
            return false;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unexpected error approving withdrawal {WithdrawalId}", withdrawalId);
            return false;
        }
    }

    public async Task<bool> ConfirmWithdrawalPaymentAsync(Guid withdrawalId)
    {
        try
        {
            var withdrawal = await db.Withdrawals.FirstOrDefaultAsync(w => w.Id == withdrawalId);
            if (withdrawal is null) throw ExceptionFactory.TransactionNotFound(withdrawalId, "Withdrawal");

            if (withdrawal.Status != PaymentStatus.Processing)
                throw ExceptionFactory.InvalidStatusTransition("withdrawal", withdrawal.Status, "confirm payment");

            return await db.ExecuteInTransactionAsync(async () =>
            {
                withdrawal.Status = PaymentStatus.Completed;
                withdrawal.UpdatedAt = DateTime.UtcNow;

                await db.SaveChangesAsync();

                logger?.LogInformation("Withdrawal {WithdrawalId} confirmed successfully", withdrawalId);
                return true;
            });
        }
        catch (TransactionNotFoundException)
        {
            return false;
        }
        catch (PaymentException ex)
        {
            logger?.LogWarning(ex, "Error confirming withdrawal payment {WithdrawalId}", withdrawalId);
            return false;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unexpected error confirming withdrawal payment {WithdrawalId}", withdrawalId);
            return false;
        }
    }

    public async Task<bool> CancelWithdrawalAsync(Guid withdrawalId)
    {
        try
        {
            var withdrawal = await db.Withdrawals
                .Include(w => w.Transaction)
                .FirstOrDefaultAsync(w => w.Id == withdrawalId);

            if (withdrawal is null) throw ExceptionFactory.TransactionNotFound(withdrawalId, "Withdrawal");

            if (withdrawal.Status == PaymentStatus.Completed)
                throw ExceptionFactory.PaymentError("Cannot cancel completed withdrawal");

            if (withdrawal.Status == PaymentStatus.Cancelled)
                throw ExceptionFactory.PaymentError("Withdrawal is already cancelled");

            // If it was already approved and being processed by the provider, we need to cancel it there
            if (withdrawal.Status == PaymentStatus.Processing)
            {
                var provider = GetWithdrawalProvider(withdrawal.Provider);
                if (provider is not null)
                    try
                    {
                        await provider.CancelWithdrawalAsync(withdrawal.ProviderTransactionId);
                    }
                    catch (Exception ex)
                    {
                        logger?.LogWarning(ex, "Failed to cancel withdrawal with provider, continuing anyway");
                    }
            }

            return await db.ExecuteInTransactionAsync(async () =>
            {
                withdrawal.Status = PaymentStatus.Cancelled;
                withdrawal.UpdatedAt = DateTime.UtcNow;

                // Create refund transaction to restore user balance
                var refundTransaction = new TransactionEntity
                {
                    Id = Guid.NewGuid(),
                    Amount = withdrawal.GemAmount, // Positive amount for refund
                    UserId = withdrawal.UserId,
                    CreatedAt = DateTime.UtcNow,
                    Type = TransactionType.Refund,
                    // No RelatedTransactionId field in TransactionEntity
                };

                db.Transactions.Add(refundTransaction);

                // Debug logs
                logger?.LogInformation("Cancelling withdrawal - GemAmount: {GemAmount}, Refund Amount: {RefundAmount}",
                    withdrawal.GemAmount, refundTransaction.Amount);

                // Update user balance
                var balance = await balanceService.GetBalanceForUpdateAsync(withdrawal.UserId);
                logger?.LogInformation("Before update - Balance Available: {BalanceAvailable}", balance.Available);

                var updatedBalance = await balanceService.UpdateBalanceAsync(balance, refundTransaction);
                logger?.LogInformation("After update - Balance Available: {BalanceAvailable}",
                    updatedBalance.Available);

                await db.SaveChangesAsync();

                // Update cache and publish balance update
                await cache.UserBalance.TrySetAsync(withdrawal.UserId, balanceMapper.ToData(updatedBalance));
                await pubSub.BalanceUpdated.TryPublishAsync(new UserUpdate<BalancePublic>
                {
                    UserId = withdrawal.UserId,
                    Mode = UpdateMode.Optimized,
                    Data = balanceMapper.ToPublic(updatedBalance),
                });

                logger?.LogInformation("Withdrawal {WithdrawalId} cancelled and funds returned", withdrawalId);
                return true;
            });
        }
        catch (TransactionNotFoundException)
        {
            return false;
        }
        catch (PaymentException ex)
        {
            logger?.LogWarning(ex, "Error cancelling withdrawal {WithdrawalId}", withdrawalId);
            return false;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unexpected error cancelling withdrawal {WithdrawalId}", withdrawalId);
            return false;
        }
    }

    public async Task<PaymentStatus> GetTransactionStatusAsync(PaymentProvider provider, string transactionId)
    {
        var paymentProvider = providerFactory.GetPaymentProvider(provider);
        if (paymentProvider == null) throw new PaymentProviderException(provider, "Payment provider not available");

        return await paymentProvider.GetTransactionStatusAsync(transactionId);
    }

    public async Task<DepositEntity?> GetDepositByProviderTransactionIdAsync(string providerTransactionId)
    {
        if (string.IsNullOrEmpty(providerTransactionId)) return null;

        return await db.Deposits
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.ProviderTransactionId == providerTransactionId);
    }

    public async Task<WithdrawalEntity?> GetWithdrawalByProviderTransactionIdAsync(string providerTransactionId)
    {
        if (string.IsNullOrEmpty(providerTransactionId)) return null;

        return await db.Withdrawals
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.ProviderTransactionId == providerTransactionId);
    }

    /// <summary>
    ///     Checks if the provider supports withdrawals
    /// </summary>
    protected virtual bool SupportsWithdrawal(PaymentProvider provider)
    {
        // All providers are now supported for withdrawals
        return true;
    }

    /// <summary>
    ///     Gets a random coin address for static wallet payment
    /// </summary>
    public async Task<string?> GetStaticWalletAddressAsync(Currency currency)
    {
        // Use a default user ID or system ID for static wallets, or get it from configuration
        var systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // System user ID
        var wallet = await walletService.GetOrCreateWalletAsync(systemUserId, currency, PaymentProvider.CryptoCloud);
        return wallet?.WalletAddress;
    }

    protected virtual IWithdrawalProvider? GetWithdrawalProvider(PaymentProvider provider)
    {
        return providerFactory.GetWithdrawalProvider(provider);
    }
}