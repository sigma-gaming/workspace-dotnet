using System.Text.Json.Serialization;
using Core.Contracts.Enums;

namespace Core.Contracts.Jsonb;

[JsonDerivedType(typeof(PromocodeBonusDepositMultiplier), typeDiscriminator: (int)PromocodeBonusType.DepositMultiplier)]
[JsonDerivedType(typeof(PromocodeBonusDepositFixed), typeDiscriminator: (int)PromocodeBonusType.DepositFixed)]
[JsonDerivedType(typeof(PromocodeBonusPayout), typeDiscriminator: (int)PromocodeBonusType.Payout)]
public abstract record PromocodeBonus
{
    public abstract PromocodeBonusType Type { get; }
};

public sealed record PromocodeBonusDepositMultiplier : PromocodeBonus
{
    public override PromocodeBonusType Type => PromocodeBonusType.DepositMultiplier;
    public int Multiplier { get; init; }
    public long? MinDeposit { get; init; }
    public long? MaxPayout { get; init; }
    public int ActiveHours { get; init; }
}

public sealed record PromocodeBonusDepositFixed : PromocodeBonus
{
    public override PromocodeBonusType Type => PromocodeBonusType.DepositFixed;
    public long Payout { get; init; }
    public long MinDeposit { get; init; }
    public int ActiveHours { get; init; }
};

public sealed record PromocodeBonusPayout : PromocodeBonus
{
    public override PromocodeBonusType Type => PromocodeBonusType.Payout;
    public long Payout { get; init; }
};