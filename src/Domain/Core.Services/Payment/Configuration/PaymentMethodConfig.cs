using Core.Contracts.Enums;

namespace Core.Services.Payment.Configuration;

/// <summary>
///     Base configuration for payment methods
/// </summary>
public abstract class PaymentMethodConfig
{
    /// <summary> Minimum amount in gems (100 = 1g) </summary>
    public required long MinAmount { get; init; }

    /// <summary> Maximum amount in gems (100 = 1g) </summary>
    public required long MaxAmount { get; init; }

    /// <summary> Commission rate (100 = 1%) </summary>
    public required int CommissionRate { get; init; }

    public required PaymentProvider Provider { get; init; }

    public required Currency Currency { get; init; }
}