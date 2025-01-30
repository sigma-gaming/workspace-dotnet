using System.Text.Json.Serialization;
using Core.Contracts.Enums;

namespace Core.Contracts.Jsonb;

[JsonDerivedType(typeof(DepositPayloadRedirect),
    typeDiscriminator: (int)DepositType.Redirect)]
[JsonDerivedType(typeof(DepositPayloadWhiteLabel),
    typeDiscriminator: (int)DepositType.WhiteLabel)]
public abstract record DepositPayload
{
    public abstract DepositType Type { get; }
}

public sealed record DepositPayloadRedirect : DepositPayload
{
    public override DepositType Type => DepositType.Redirect;
    public required string RedirectUrl { get; init; }
}

public sealed record DepositPayloadWhiteLabel : DepositPayload
{
    public override DepositType Type => DepositType.WhiteLabel;
    public required DepositInstructions Instructions { get; init; }
}
