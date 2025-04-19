using Core.Contracts.Enums;
using Core.Services.Payment.Exceptions;
using Microsoft.Extensions.Logging;

namespace Core.Services.Payment;

/// <summary>
/// Handler for payment provider webhooks
/// </summary>
public class PaymentWebhookHandler(
    PaymentService paymentService,
    ILogger<PaymentWebhookHandler> logger)
{
    /// <summary>
    /// Handles a deposit webhook notification
    /// </summary>
    /// <param name="providerTransactionId">Provider's transaction ID</param>
    /// <param name="status">Payment status</param>
    /// <param name="provider">Payment provider</param>
    /// <returns>True if the notification was handled successfully</returns>
    public async Task<bool> HandleDepositWebhookAsync(
        string providerTransactionId,
        PaymentStatus status,
        PaymentProvider provider)
    {
        try
        {
            // Get the deposit by provider transaction ID
            var deposit = await paymentService.GetDepositByProviderTransactionIdAsync(providerTransactionId);

            if (deposit == null)
            {
                logger.LogWarning("Deposit with provider transaction ID {TransactionId} not found",
                    providerTransactionId);
                throw new TransactionNotFoundException(providerTransactionId, "Deposit");
            }

            // Handle the notification based on the status
            switch (status)
            {
                case PaymentStatus.Completed:
                    return await paymentService.ConfirmDepositAsync(deposit.Id);

                case PaymentStatus.Failed:
                case PaymentStatus.Cancelled:
                case PaymentStatus.Rejected:
                case PaymentStatus.Expired:
                    return await paymentService.CancelDepositAsync(deposit.Id);

                default:
                    logger.LogInformation("Ignoring deposit webhook with status {Status}", status);
                    return true;
            }
        }
        catch (TransactionNotFoundException ex)
        {
            logger.LogWarning(ex, "Transaction not found in deposit webhook handler: {Message}", ex.Message);
            return false;
        }
        catch (PaymentException ex)
        {
            logger.LogWarning(ex, "Payment error in deposit webhook handler: {Message}", ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle deposit webhook for transaction {TransactionId}",
                providerTransactionId);
            return false;
        }
    }

    /// <summary>
    /// Handles a withdrawal webhook notification
    /// </summary>
    /// <param name="providerTransactionId">Provider's transaction ID</param>
    /// <param name="status">Payment status</param>
    /// <param name="provider">Payment provider</param>
    /// <returns>True if the notification was handled successfully</returns>
    public async Task<bool> HandleWithdrawalWebhookAsync(
        string providerTransactionId,
        PaymentStatus status,
        PaymentProvider provider)
    {
        try
        {
            // Get the withdrawal by provider transaction ID
            var withdrawal = await paymentService.GetWithdrawalByProviderTransactionIdAsync(providerTransactionId);

            if (withdrawal == null)
            {
                logger.LogWarning("Withdrawal with provider transaction ID {TransactionId} not found",
                    providerTransactionId);
                throw new TransactionNotFoundException(providerTransactionId, "Withdrawal");
            }

            // Handle the notification based on the status
            switch (status)
            {
                case PaymentStatus.Completed:
                    return await paymentService.ConfirmWithdrawalPaymentAsync(withdrawal.Id);

                case PaymentStatus.Failed:
                case PaymentStatus.Cancelled:
                case PaymentStatus.Rejected:
                case PaymentStatus.Expired:
                    return await paymentService.CancelWithdrawalAsync(withdrawal.Id);

                default:
                    logger.LogInformation("Ignoring withdrawal webhook with status {Status}", status);
                    return true;
            }
        }
        catch (TransactionNotFoundException ex)
        {
            logger.LogWarning(ex, "Transaction not found in withdrawal webhook handler: {Message}", ex.Message);
            return false;
        }
        catch (PaymentException ex)
        {
            logger.LogWarning(ex, "Payment error in withdrawal webhook handler: {Message}", ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle withdrawal webhook for transaction {TransactionId}",
                providerTransactionId);
            return false;
        }
    }
}
