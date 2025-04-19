using Core.Contracts.Enums;

namespace Core.Services.Payment.Interfaces;

/// <summary>
/// Interface for payment providers that support static wallets
/// </summary>
public interface IStaticWalletProvider : IPaymentProviderService
{
    /// <summary>
    /// Checks if the provider supports static wallets for the specified currency
    /// </summary>
    /// <param name="currency">Currency to check</param>
    /// <returns>True if static wallets are supported for this currency</returns>
    bool SupportsStaticWallets(Currency currency);
    
    /// <summary>
    /// Generates a new static wallet address for the user
    /// </summary>
    /// <param name="currency">Currency for the wallet</param>
    /// <param name="userId">User ID</param>
    /// <returns>Generated wallet address</returns>
    Task<string> GenerateWalletAddressAsync(Currency currency, Guid userId);
}
