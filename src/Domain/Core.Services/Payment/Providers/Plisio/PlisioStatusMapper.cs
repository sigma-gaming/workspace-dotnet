using Core.Contracts.Enums;

namespace Core.Services.Payment.Providers.Plisio;

/// <summary>
///     Maps Plisio statuses to PaymentStatus
/// </summary>
public static class PlisioStatusMapper
{
    /// <summary>
    ///     Maps Plisio invoice status to PaymentStatus
    /// </summary>
    public static PaymentStatus MapInvoiceStatus(string plisioStatus)
    {
        return plisioStatus.ToLower() switch
        {
            "new" => PaymentStatus.Pending,
            "pending" => PaymentStatus.Processing,
            "completed" => PaymentStatus.Completed,
            "mismatch" => PaymentStatus.Failed,
            "expired" => PaymentStatus.Expired,
            "cancelled" => PaymentStatus.Cancelled,
            "error" => PaymentStatus.Failed,
            _ => PaymentStatus.Failed,
        };
    }

    /// <summary>
    ///     Maps Plisio withdrawal status to PaymentStatus
    /// </summary>
    public static PaymentStatus MapWithdrawalStatus(string plisioStatus)
    {
        // Add null check to avoid NullReferenceException
        if (string.IsNullOrEmpty(plisioStatus))
            return PaymentStatus.Pending; // Default status when null or empty

        return plisioStatus.ToLower() switch
        {
            "new" => PaymentStatus.Pending,
            "pending" => PaymentStatus.Processing,
            "completed" => PaymentStatus.Completed,
            "error" => PaymentStatus.Failed,
            _ => PaymentStatus.Failed,
        };
    }
}