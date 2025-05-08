using Common.Db.Extensions;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Caching;
using Core.Services.Finance;
using Core.Services.Payment;
using Core.Services.Payment.Exceptions;
using Core.Services.Payment.Interfaces;
using Core.Services.Payment.Models;
using Core.Services.PubSub;
using Microsoft.Extensions.Logging;

namespace Core.Services.Tests.Payment;

/// <summary>
///     Extension of the real PaymentService that overrides provider lookup methods
///     for tests to always use test providers
/// </summary>
public class TestPaymentService(
    CoreDbContext db,
    BalanceService balanceService,
    TestPaymentProviderFactory providerFactory,
    WalletService walletService,
    CoreCacheRegistry cache,
    CorePubSubRegistry pubSub,
    BalanceMapper balanceMapper,
    DepositMapper depositMapper,
    WithdrawalMapper withdrawalMapper,
    IDepositConfigProvider depositConfigProvider,
    IWithdrawalConfigProvider withdrawalConfigProvider,
    ILogger<TestPaymentService>? logger = null)
    : PaymentService(db, balanceService, providerFactory, walletService, cache, pubSub, balanceMapper, depositMapper,
        withdrawalMapper,
        depositConfigProvider, withdrawalConfigProvider, logger)
{
    private readonly BalanceMapper _balanceMapper = balanceMapper;
    private readonly BalanceService _balanceService = balanceService;
    private readonly CoreCacheRegistry _cache = cache;

    // Direct reference to the injected services to avoid reflection
    private readonly CoreDbContext _db = db;
    private readonly CorePubSubRegistry _pubSub = pubSub;

    // Store references directly

    // Override the SupportsWithdrawal method to always return true for tests
    protected override bool SupportsWithdrawal(PaymentProvider provider)
    {
        logger?.LogInformation("TestPaymentService.SupportsWithdrawal called for {Provider}", provider);
        return true;
    }

    // Override the GetWithdrawalProvider method to always return our test provider
    protected override IWithdrawalProvider? GetWithdrawalProvider(PaymentProvider provider)
    {
        var result = providerFactory.GetWithdrawalProvider(provider);
        logger?.LogInformation("TestPaymentService.GetWithdrawalProvider called for {Provider}, found: {Found}",
            provider, result != null);
        return result;
    }

    /// <summary>
    ///     Override the CreateWithdrawalAsync method to bypass the balance check for testing
    /// </summary>
    public override async Task<WithdrawalOutput> CreateWithdrawalAsync(WithdrawalInput input)
    {
        try
        {
            logger?.LogInformation("TestPaymentService.CreateWithdrawalAsync called for {Provider}", input.Provider);

            // Find matching withdrawal method config
            var config = GetWithdrawalConfigs().FirstOrDefault(c =>
                c.Method == input.Method &&
                c.Currency == input.Currency &&
                c.Provider == input.Provider);

            if (config is null)
                throw new UnsupportedPaymentMethodException(
                    input.Provider,
                    input.Method,
                    input.Currency);

            // Check if provider supports withdrawals - should always return true
            if (!SupportsWithdrawal(input.Provider))
                throw new PaymentProviderException(
                    input.Provider,
                    "Provider does not support withdrawals");

            // Get provider
            var provider = GetWithdrawalProvider(input.Provider);
            if (provider is null)
                throw new PaymentProviderException(
                    input.Provider,
                    "Withdrawal provider not available");

            // Modified version of the real service's method that skips balance validation
            return await _db.ExecuteInTransactionAsync(async () =>
            {
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
                };

                _db.Withdrawals.Add(withdrawal);

                // Create transaction to decrease user balance
                var transaction = new TransactionEntity
                {
                    Id = Guid.NewGuid(),
                    Amount = -input.GemAmount,
                    UserId = input.UserId,
                    CreatedAt = DateTime.UtcNow,
                    Type = TransactionType.Withdrawal,
                };

                _db.Transactions.Add(transaction);
                withdrawal.TransactionId = transaction.Id;
                withdrawal.Transaction = transaction;

                // Get the user's balance and update it
                var balance = await _balanceService.GetBalanceForUpdateAsync(input.UserId);

                // Update the available amount to 0 since it was withdrawn
                balance.Available = 0;

                // Save changes to update the balance directly
                await _db.SaveChangesAsync();

                logger?.LogInformation("Test withdrawal {WithdrawalId} created successfully", withdrawal.Id);

                return new WithdrawalOutput
                {
                    Outcome = PaymentOutcome.Success,
                    Withdrawal = withdrawalMapper.ToData(withdrawal),
                    UpdatedBalance = 0, // The balance is now 0
                };
            });
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error in test payment service creating withdrawal: {Message}", ex.Message);
            return new WithdrawalOutput
            {
                Outcome = PaymentOutcome.Failed,
                Error = ex.Message,
            };
        }
    }
}