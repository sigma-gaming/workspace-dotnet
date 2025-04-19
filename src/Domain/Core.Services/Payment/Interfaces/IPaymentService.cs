using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Services.Payment.Configuration;
using Core.Services.Payment.Models;

namespace Core.Services.Payment.Interfaces;

/// <summary>
/// Interface for Payment Service with clear separation of concerns
/// </summary>
public interface IPaymentService
{
    // Configuration methods
    List<DepositMethodConfig> GetDepositConfigs();
    List<WithdrawalMethodConfig> GetWithdrawalConfigs();
    
    // Deposit operations
    Task<DepositOutput> CreateDepositAsync(DepositInput input);
    Task<bool> ConfirmDepositAsync(Guid depositId);
    Task<bool> CancelDepositAsync(Guid depositId);
    Task<DepositEntity?> GetDepositByProviderTransactionIdAsync(string providerTransactionId);
    
    // Withdrawal operations
    Task<WithdrawalOutput> CreateWithdrawalAsync(WithdrawalInput input);
    Task<bool> ApproveWithdrawalAsync(Guid withdrawalId, Guid adminId);
    Task<bool> ConfirmWithdrawalPaymentAsync(Guid withdrawalId);
    Task<bool> CancelWithdrawalAsync(Guid withdrawalId);
    Task<WithdrawalEntity?> GetWithdrawalByProviderTransactionIdAsync(string providerTransactionId);
    
    // General payment operations
    Task<PaymentStatus> GetTransactionStatusAsync(PaymentProvider provider, string transactionId);
} 