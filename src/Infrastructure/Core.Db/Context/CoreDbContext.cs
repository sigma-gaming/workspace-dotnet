using System.Reflection;
using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Db.Context;

public class CoreDbContext : DbContext
{
    public CoreDbContext()
    {
    }

    public CoreDbContext(DbContextOptions<CoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountEntity> Accounts { get; set; }

    public virtual DbSet<BalanceEntity> Balances { get; set; }

    public virtual DbSet<BudgetEntity> Budget { get; set; }

    public virtual DbSet<ChatMessageEntity> ChatMessages { get; set; }

    public virtual DbSet<ConfigEntity> Config { get; set; }

    public virtual DbSet<DepositEntity> Deposits { get; set; }

    public virtual DbSet<DomainEntity> Domains { get; set; }

    public virtual DbSet<GameRecordEntity> GameRecords { get; set; }

    public virtual DbSet<GlobalTaskEntity> GlobalTasks { get; set; }

    public virtual DbSet<GlobalTaskStatusEntity> GlobalTaskStatuses { get; set; }

    public virtual DbSet<NotificationEntity> Notifications { get; set; }

    public virtual DbSet<ProfileEntity> Profiles { get; set; }

    public virtual DbSet<PromocodeEntity> Promocodes { get; set; }

    public virtual DbSet<PromocodeUsageEntity> PromocodeUsages { get; set; }

    public virtual DbSet<ReferralCampaignEntity> ReferralCampaigns { get; set; }

    public virtual DbSet<ReferrerBalanceEntity> ReferrerBalances { get; set; }

    public virtual DbSet<ReferrerPayoutEntity> ReferrerPayouts { get; set; }

    public virtual DbSet<ReferrerSettingsEntity> ReferrerSettings { get; set; }

    public virtual DbSet<ReferrerTransactionEntity> ReferrerTransactions { get; set; }

    public virtual DbSet<ReferrerWithdrawalEntity> ReferrerWithdrawals { get; set; }

    public virtual DbSet<SessionEntity> Sessions { get; set; }

    public virtual DbSet<TransactionEntity> Transactions { get; set; }

    public virtual DbSet<UserEntity> Users { get; set; }

    public virtual DbSet<UserSecurityEntity> UserSecurities { get; set; }

    public virtual DbSet<UserStatsEntity> UserStats { get; set; }

    public virtual DbSet<WithdrawalEntity> Withdrawals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigEntity>().HasData(
            new ConfigEntity
            {
                Id = 1,
                MaintenanceEnabled = false,
                BackgroundJobsEnabled = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            });

        modelBuilder.Entity<BudgetEntity>().HasData(
            new BudgetEntity
            {
                Id = 1,
                Available = 100000_00,
                LastSyncAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            });

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}