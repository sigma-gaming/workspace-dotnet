using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Core.Contracts.Enums;

namespace Core.Contracts.Jsonb;

[JsonDerivedType(typeof(DepositInstructionsCryptoWallet),
    (int)DepositInstructionsType.CryptoWallet)]
public abstract record DepositInstructions
{
    [Required] public abstract DepositInstructionsType Type { get; }
}

public sealed record DepositInstructionsCryptoWallet : DepositInstructions
{
    [Required] public override DepositInstructionsType Type => DepositInstructionsType.CryptoWallet;
    public required string Address { get; init; }
    public required Currency Currency { get; init; }
    public required decimal Amount { get; init; }
}
