using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Services.Payment.Interfaces;
using Core.Services.Payment.Models;
using Microsoft.Extensions.Logging;

namespace Core.Services.Tests.Payment;

/// <summary>
///     Test deposit provider implementation for tests
/// </summary>
public class TestDepositProvider(ILogger<TestDepositProvider>? logger = null) : IDepositProvider, ITestPaymentProvider
{
    public PaymentProvider Provider => PaymentProvider.Plisio;

    public Task<DepositResponse> CreateDepositAsync(DepositInput input)
    {
        logger?.LogInformation("TestDepositProvider.CreateDepositAsync called for {Provider}", input.Provider);

        // Always return a successful response for testing
        return Task.FromResult(new DepositResponse
        {
            ProviderTransactionId = Guid.NewGuid().ToString(),
            Status = PaymentStatus.Pending,
            ProviderAmount = "1.0",
            Currency = input.Currency,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Type = DepositType.Redirect,
            Payload = new DepositPayloadRedirect { RedirectUrl = "https://example.com" },
        });
    }

    public Task<PaymentStatus> GetDepositStatusAsync(string providerTransactionId)
    {
        return Task.FromResult(PaymentStatus.Completed);
    }

    public Task CancelDepositAsync(string providerTransactionId)
    {
        return Task.CompletedTask;
    }

    public Task<PaymentStatus> GetTransactionStatusAsync(string transactionId)
    {
        return Task.FromResult(PaymentStatus.Completed);
    }

    public string TestIdentifier => "DepositTest";
}

/// <summary>
///     Test withdrawal provider implementation for tests
/// </summary>
public class TestWithdrawalProvider(ILogger<TestWithdrawalProvider>? logger = null) : IWithdrawalProvider, ITestPaymentProvider
{
    public string TestIdentifier => "WithdrawalTest";
    
    // Override Provider behavior to accept any provider
    // Note: Using private field to store the actual provider used by the last request
    private PaymentProvider _provider = PaymentProvider.Plisio;
    
    public PaymentProvider Provider => _provider;

    public Task<WithdrawalResponse> CreateWithdrawalAsync(WithdrawalInput input)
    {
        logger?.LogInformation("TestWithdrawalProvider.CreateWithdrawalAsync called for {Provider} with amount {Amount}", 
            input.Provider, input.GemAmount);
            
        // Store the requested provider for future reference
        _provider = input.Provider;
        
        logger?.LogInformation("Now supporting provider: {Provider}", _provider);

        // Always return a successful response for testing but use a reasonable string value
        // for the provider amount to avoid numeric overflow in the database
        return Task.FromResult(new WithdrawalResponse
        {
            ProviderTransactionId = Guid.NewGuid().ToString(),
            Status = PaymentStatus.Pending,
            ProviderAmount = "1.0", // Use a fixed value instead of the raw input amount
            Currency = input.Currency,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        });
    }

    public Task<PaymentStatus> GetWithdrawalStatusAsync(string providerTransactionId)
    {
        logger?.LogInformation("TestWithdrawalProvider.GetWithdrawalStatusAsync called for {ProviderTransactionId}", 
            providerTransactionId);
        return Task.FromResult(PaymentStatus.Pending);
    }

    public Task CancelWithdrawalAsync(string providerTransactionId)
    {
        logger?.LogInformation("TestWithdrawalProvider.CancelWithdrawalAsync called for {ProviderTransactionId}", 
            providerTransactionId);
        return Task.CompletedTask;
    }

    public Task<PaymentStatus> GetTransactionStatusAsync(string transactionId)
    {
        logger?.LogInformation("TestWithdrawalProvider.GetTransactionStatusAsync called for {TransactionId}", 
            transactionId);
        return Task.FromResult(PaymentStatus.Pending);
    }
}