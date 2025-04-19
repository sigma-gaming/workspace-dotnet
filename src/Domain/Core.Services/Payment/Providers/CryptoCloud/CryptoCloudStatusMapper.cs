using Core.Contracts.Enums;

namespace Core.Services.Payment.Providers.CryptoCloud;

/// <summary>
/// Maps CryptoCloud statuses to PaymentStatus
/// </summary>
public static class CryptoCloudStatusMapper
{
    /// <summary>
    /// Maps CryptoCloud wallet status to PaymentStatus
    /// </summary>
    public static PaymentStatus MapWalletStatus(string cryptoCloudStatus)
    {
        return cryptoCloudStatus.ToLower() switch
        {
            "pending" => PaymentStatus.Pending,
            "processing" => PaymentStatus.Processing,
            "completed" => PaymentStatus.Completed,
            "confirmed" => PaymentStatus.Completed,
            "failed" => PaymentStatus.Failed,
            "expired" => PaymentStatus.Expired,
            "cancelled" => PaymentStatus.Cancelled,
            _ => PaymentStatus.Pending
        };
    }
}
