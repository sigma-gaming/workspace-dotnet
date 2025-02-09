namespace Core.Contracts.Entities;

public sealed record BudgetEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime LastSyncAt { get; set; }
    public long Available { get; set; }
}