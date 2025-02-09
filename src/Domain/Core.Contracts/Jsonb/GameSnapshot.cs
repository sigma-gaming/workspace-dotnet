using System.Text.Json.Serialization;
using Core.Contracts.Enums;

namespace Core.Contracts.Jsonb;

[JsonDerivedType(typeof(GameSnapshotDice), (int)Game.Dice)]
[JsonDerivedType(typeof(GameSnapshotPincode), (int)Game.Pincode)]
public abstract record GameSnapshot
{
    public abstract Game Game { get; }
}

public sealed record GameSnapshotDice : GameSnapshot
{
    public override Game Game => Game.Dice;
    public required int[] InputSides { get; init; }
    public required int OutputSide { get; init; }
}

public sealed record GameSnapshotPincode : GameSnapshot
{
    public override Game Game => Game.Pincode;
    public required PincodeMode Mode { get; init; }
    public required int OutputNumber { get; init; }
}