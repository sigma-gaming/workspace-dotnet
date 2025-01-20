using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Core.Contracts.Entities;

namespace Core.Database.Context;

public class CoreDbContext : DbContext
{
    public CoreDbContext()
    {
    }

    public CoreDbContext(DbContextOptions<CoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Balance> Balances { get; set; }

    public virtual DbSet<Budget> Budget { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<Config> Config { get; set; }

    public virtual DbSet<Deposit> Deposits { get; set; }

    public virtual DbSet<Contracts.Entities.Domain> Domains { get; set; }

    public virtual DbSet<GameRecord> GameRecords { get; set; }

    public virtual DbSet<GlobalTask> GlobalTasks { get; set; }

    public virtual DbSet<GlobalTaskStatus> GlobalTaskStatuses { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Promocode> Promocodes { get; set; }

    public virtual DbSet<PromocodeUsage> PromocodeUsages { get; set; }

    public virtual DbSet<ReferralCampaign> ReferralCampaigns { get; set; }

    public virtual DbSet<ReferrerBalance> ReferrerBalances { get; set; }

    public virtual DbSet<ReferrerPayout> ReferrerPayouts { get; set; }

    public virtual DbSet<ReferrerSettings> ReferrerSettings { get; set; }

    public virtual DbSet<ReferrerTransaction> ReferrerTransactions { get; set; }

    public virtual DbSet<ReferrerWithdrawal> ReferrerWithdrawals { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSecurity> UserSecurities { get; set; }

    public virtual DbSet<UserStats> UserStats { get; set; }

    public virtual DbSet<Withdrawal> Withdrawals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Config>().HasData(
            new Config
            {
                Id = 1,
                MaintenanceEnabled = false,
                BackgroundJobsEnabled = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

        modelBuilder.Entity<Budget>().HasData(
            new Budget
            {
                Id = 1,
                Available = 100000_00,
                LastSyncAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}