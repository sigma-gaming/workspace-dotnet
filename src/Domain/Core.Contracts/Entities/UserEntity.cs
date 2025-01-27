using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record UserEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? ProfileId { get; set; }
    public Guid? SecurityId { get; set; }
    public Guid? ReferrerId { get; set; }
    public Guid? ReferralCampaignId { get; set; }
    public bool Virtual { get; set; } = false;
    public UserRole[] Roles { get; set; } = [];
    public ICollection<AccountEntity> Accounts { get; set; } = new List<AccountEntity>();
    public BalanceEntity? Balance { get; set; }
    public UserEntity? Referrer { get; set; }
    public ProfileEntity? Profile { get; set; }
    public UserSecurityEntity? UserSecurity { get; set; }
    public UserStatsEntity? UserStats { get; set; }
    public ReferrerBalanceEntity? ReferrerBalance { get; set; }
    public ReferrerPayoutEntity? ReferrerPayout { get; set; }
    public ReferrerSettingsEntity? ReferrerSettings { get; set; }
    public ICollection<ChatMessageEntity> ChatMessages { get; set; } = new List<ChatMessageEntity>();
    public ICollection<DepositEntity> Deposits { get; set; } = new List<DepositEntity>();
    public ICollection<GameRecordEntity> GameRecords { get; set; } = new List<GameRecordEntity>();
    public ICollection<UserEntity> InverseReferrer { get; set; } = new List<UserEntity>();
    public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();
    public ICollection<PromocodeEntity> PromocodeCreatedByNavigations { get; set; } = new List<PromocodeEntity>();
    public ICollection<PromocodeUsageEntity> PromocodeUsages { get; set; } = new List<PromocodeUsageEntity>();
    public ICollection<PromocodeEntity> PromocodeUsers { get; set; } = new List<PromocodeEntity>();
    public ICollection<ReferralCampaignEntity> ReferralCampaigns { get; set; } = new List<ReferralCampaignEntity>();

    public ICollection<ReferrerTransactionEntity> ReferrerTransactionReferrals { get; set; } =
        new List<ReferrerTransactionEntity>();

    public ICollection<ReferrerTransactionEntity> ReferrerTransactionReferrers { get; set; } =
        new List<ReferrerTransactionEntity>();

    public ICollection<ReferrerWithdrawalEntity> ReferrerWithdrawals { get; set; } =
        new List<ReferrerWithdrawalEntity>();

    public ICollection<SessionEntity> SessionReferrers { get; set; } = new List<SessionEntity>();
    public ICollection<SessionEntity> SessionUsers { get; set; } = new List<SessionEntity>();
    public ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
    public ICollection<WithdrawalEntity> Withdrawals { get; set; } = new List<WithdrawalEntity>();
}