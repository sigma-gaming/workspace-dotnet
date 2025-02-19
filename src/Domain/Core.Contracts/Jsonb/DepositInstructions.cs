using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Core.Contracts.Enums;

namespace Core.Contracts.Jsonb;

[JsonDerivedType(typeof(DepositInstructionsSbp),
    (int)DepositMethod.Sbp)]
public abstract record DepositInstructions
{
    [Required] public abstract DepositMethod Method { get; }
}

public sealed record DepositInstructionsSbp : DepositInstructions
{
    [Required] public override DepositMethod Method => DepositMethod.Sbp;
    public required string PhoneNumber { get; init; }
    public required string ProviderAmount { get; init; }
    public string? RecipientName { get; init; }
    public int? ExpiresAt { get; init; }
}