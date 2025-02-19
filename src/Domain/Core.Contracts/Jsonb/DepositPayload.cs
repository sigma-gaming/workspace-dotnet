using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Core.Contracts.Enums;

namespace Core.Contracts.Jsonb;

[JsonDerivedType(typeof(DepositPayloadRedirect),
    (int)DepositType.Redirect)]
[JsonDerivedType(typeof(DepositPayloadWhiteLabel),
    (int)DepositType.WhiteLabel)]
public abstract record DepositPayload
{
    [Required] public abstract DepositType Type { get; }
}

public sealed record DepositPayloadRedirect : DepositPayload
{
    [Required] public override DepositType Type => DepositType.Redirect;
    public required string RedirectUrl { get; init; }
}

public sealed record DepositPayloadWhiteLabel : DepositPayload
{
    [Required] public override DepositType Type => DepositType.WhiteLabel;
    public required DepositInstructions Instructions { get; init; }
}