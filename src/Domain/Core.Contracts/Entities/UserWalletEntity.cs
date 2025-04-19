using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record UserWalletEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public PaymentProvider Provider { get; set; }
    public Currency Currency { get; set; }
    public string WalletAddress { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    
    // Navigation properties
    public UserEntity User { get; set; } = null!;
}
