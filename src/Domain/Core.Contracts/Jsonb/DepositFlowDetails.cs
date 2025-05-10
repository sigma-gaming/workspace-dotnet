using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Core.Contracts.Enums;

namespace Core.Contracts.Jsonb;

[JsonDerivedType(typeof(DepositFlowDetailsRedirect),
    (int)DepositFlow.Redirect)]
[JsonDerivedType(typeof(DepositFlowDetailsWhiteLabel),
    (int)DepositFlow.WhiteLabel)]
public abstract record DepositFlowDetails
{
    [Required] public abstract DepositFlow Flow { get; }
}

public sealed record DepositFlowDetailsRedirect : DepositFlowDetails
{
    [Required] public override DepositFlow Flow => DepositFlow.Redirect;
    public required string RedirectUrl { get; init; }
}

public sealed record DepositFlowDetailsWhiteLabel : DepositFlowDetails
{
    [Required] public override DepositFlow Flow => DepositFlow.WhiteLabel;
    public required DepositInstructions Instructions { get; init; }
}