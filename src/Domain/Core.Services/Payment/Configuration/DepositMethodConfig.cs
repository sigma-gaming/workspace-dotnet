using Core.Contracts.Enums;

namespace Core.Services.Payment.Configuration;

/// <summary>
///     Configuration for deposit methods
/// </summary>
public class DepositMethodConfig : PaymentMethodConfig
{
    public required DepositMethod Method { get; init; }
}