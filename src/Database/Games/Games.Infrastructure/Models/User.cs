using System;
using System.Collections.Generic;
using Games.Domain.Enums;

namespace Games.Infrastructure.Models;

public partial class User
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

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual Balance? Balance { get; set; }

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();

    public virtual ICollection<GameRecord> GameRecords { get; set; } = new List<GameRecord>();

    public virtual ICollection<User> InverseReferrer { get; set; } = new List<User>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Profile? Profile { get; set; }

    public virtual ICollection<Promocode> PromocodeCreatedByNavigations { get; set; } = new List<Promocode>();

    public virtual ICollection<PromocodeUsage> PromocodeUsages { get; set; } = new List<PromocodeUsage>();

    public virtual ICollection<Promocode> PromocodeUsers { get; set; } = new List<Promocode>();

    public virtual ICollection<ReferralCampaign> ReferralCampaigns { get; set; } = new List<ReferralCampaign>();

    public virtual User? Referrer { get; set; }

    public virtual ReferrerBalance? ReferrerBalance { get; set; }

    public virtual ReferrerPayout? ReferrerPayout { get; set; }

    public virtual ReferrerSetting? ReferrerSetting { get; set; }

    public virtual ICollection<ReferrerTransaction> ReferrerTransactionReferrals { get; set; } = new List<ReferrerTransaction>();

    public virtual ICollection<ReferrerTransaction> ReferrerTransactionReferrers { get; set; } = new List<ReferrerTransaction>();

    public virtual ICollection<ReferrerWithdrawal> ReferrerWithdrawals { get; set; } = new List<ReferrerWithdrawal>();

    public virtual ICollection<Session> SessionReferrers { get; set; } = new List<Session>();

    public virtual ICollection<Session> SessionUsers { get; set; } = new List<Session>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual UserSecurity? UserSecurity { get; set; }

    public virtual UserStat? UserStat { get; set; }

    public virtual ICollection<Withdrawal> Withdrawals { get; set; } = new List<Withdrawal>();
}
