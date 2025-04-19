namespace Core.Services.Payment.Providers.Plisio.Models;

/// <summary>
///     Request to create a new invoice
/// </summary>
public class PlisioCreateInvoiceRequest
{
    /// <summary>
    ///     One of the cryptocurrencies supported by Plisio
    /// </summary>
    public string Currency { get; set; } = null!;

    /// <summary>
    ///     Cryptocurrency amount (optional if source_currency and source_amount are provided)
    /// </summary>
    public string? Amount { get; set; }

    /// <summary>
    ///     Fiat currency (optional)
    /// </summary>
    public string? SourceCurrency { get; set; }

    /// <summary>
    ///     Fiat amount (optional)
    /// </summary>
    public string? SourceAmount { get; set; }

    /// <summary>
    ///     Comma-separated list of cryptocurrencies that allowed for payment
    /// </summary>
    public string? AllowedPsysCids { get; set; }

    /// <summary>
    ///     Merchant internal order name
    /// </summary>
    public string OrderName { get; set; } = null!;

    /// <summary>
    ///     Merchant internal order number (must be unique)
    /// </summary>
    public string OrderNumber { get; set; } = null!;

    /// <summary>
    ///     Merchant invoice description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Merchant full URL to get invoice updates
    /// </summary>
    public string? CallbackUrl { get; set; }

    /// <summary>
    ///     Interval in minutes when invoice will be expired
    /// </summary>
    public string? ExpireMin { get; set; }
}

/// <summary>
///     Request to create a withdrawal
/// </summary>
public class PlisioCreateWithdrawalRequest
{
    /// <summary>
    ///     One of the cryptocurrencies supported by Plisio
    /// </summary>
    public string PsysCid { get; set; } = null!;

    /// <summary>
    ///     Hash or multiple comma separated hashes
    /// </summary>
    public string To { get; set; } = null!;

    /// <summary>
    ///     Any comma separated float values
    /// </summary>
    public string Amount { get; set; } = null!;

    /// <summary>
    ///     'cash_out' or 'mass_cash_out'
    /// </summary>
    public string Type { get; set; } = "cash_out";

    /// <summary>
    ///     Normal or priority
    /// </summary>
    public string? FeePlan { get; set; }
}