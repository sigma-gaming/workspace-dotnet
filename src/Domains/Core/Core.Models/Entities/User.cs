using Core.Domain.Enums;

namespace Core.Models.Entities;

public sealed record User
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int? ProfileId { get; set; }

    public Guid? SecurityId { get; set; }

    public Guid? ReferrerId { get; set; }

    public Guid? ReferralCampaignId { get; set; }

    public bool Virtual { get; set; }

    public UserRole[] Roles { get; set; } = Array.Empty<UserRole>();

    public ICollection<Account> Accounts { get; set; } = new List<Account>();

    public Balance? Balance { get; set; }

    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();

    public ICollection<GameRecord> GameRecords { get; set; } = new List<GameRecord>();

    public ICollection<User> InverseReferrer { get; set; } = new List<User>();

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public Profile? Profile { get; set; }

    public ICollection<Promocode> PromocodeCreatedByNavigations { get; set; } = new List<Promocode>();

    public ICollection<PromocodeUsage> PromocodeUsages { get; set; } = new List<PromocodeUsage>();

    public ICollection<Promocode> PromocodeUsers { get; set; } = new List<Promocode>();

    public ICollection<ReferralCampaign> ReferralCampaigns { get; set; } = new List<ReferralCampaign>();

    public User? Referrer { get; set; }

    public ReferrerBalance? ReferrerBalance { get; set; }

    public ReferrerPayout? ReferrerPayout { get; set; }

    public ReferrerSettings? ReferrerSettings { get; set; }

    public ICollection<ReferrerTransaction> ReferrerTransactionReferrals { get; set; } = new List<ReferrerTransaction>();

    public ICollection<ReferrerTransaction> ReferrerTransactionReferrers { get; set; } = new List<ReferrerTransaction>();

    public ICollection<ReferrerWithdrawal> ReferrerWithdrawals { get; set; } = new List<ReferrerWithdrawal>();

    public ICollection<Session> SessionReferrers { get; set; } = new List<Session>();

    public ICollection<Session> SessionUsers { get; set; } = new List<Session>();

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public UserSecurity? UserSecurity { get; set; }

    public UserStats? UserStats { get; set; }

    public ICollection<Withdrawal> Withdrawals { get; set; } = new List<Withdrawal>();
}
