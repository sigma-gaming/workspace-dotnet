using Microsoft.EntityFrameworkCore;
using Games.Infrastructure.Models;
using Games.Domain.Enums;

namespace Games.Infrastructure.Context;

public partial class GamesDbContext : DbContext
{
    public GamesDbContext()
    {
    }

    public GamesDbContext(DbContextOptions<GamesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Balance> Balances { get; set; }

    public virtual DbSet<Budget> Budgets { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<Config> Configs { get; set; }

    public virtual DbSet<Deposit> Deposits { get; set; }

    public virtual DbSet<Models.Domain> Domains { get; set; }

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

    public virtual DbSet<ReferrerSetting> ReferrerSettings { get; set; }

    public virtual DbSet<ReferrerTransaction> ReferrerTransactions { get; set; }

    public virtual DbSet<ReferrerWithdrawal> ReferrerWithdrawals { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSecurity> UserSecurities { get; set; }

    public virtual DbSet<UserStat> UserStats { get; set; }

    public virtual DbSet<Withdrawal> Withdrawals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum<AccountProvider>()
            .HasPostgresEnum<ChatMessageType>()
            .HasPostgresEnum<Currency>()
            .HasPostgresEnum<DepositMethod>()
            .HasPostgresEnum<DepositType>()
            .HasPostgresEnum<DomainApp>()
            .HasPostgresEnum<FraudRisk>()
            .HasPostgresEnum<Game>()
            .HasPostgresEnum<GameOutcome>()
            .HasPostgresEnum<GlobalTaskKey>()
            .HasPostgresEnum<NotificationKind>()
            .HasPostgresEnum<PaymentProvider>()
            .HasPostgresEnum<PaymentStatus>()
            .HasPostgresEnum<PromocodeBonusType>()
            .HasPostgresEnum<PromocodeUsageStatus>()
            .HasPostgresEnum<ReferralAction>()
            .HasPostgresEnum<Domain.Enums.TaskStatus>()
            .HasPostgresEnum<TaskType>()
            .HasPostgresEnum<TransactionType>()
            .HasPostgresEnum<UserRole>()
            .HasPostgresEnum<WithdrawalMethod>()
            .HasPostgresExtension("pg_uuidv7");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Account_pkey");

            entity.ToTable("Account");

            entity.HasIndex(e => e.ProviderUserId, "Account_providerUserId_index");

            entity.HasIndex(e => e.UserId, "Account_userId_index");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.ProviderUserFirstName).HasColumnName("providerUserFirstName");
            entity.Property(e => e.ProviderUserId).HasColumnName("providerUserId");
            entity.Property(e => e.ProviderUserImage).HasColumnName("providerUserImage");
            entity.Property(e => e.ProviderUserLastName).HasColumnName("providerUserLastName");
            entity.Property(e => e.ProviderUsername).HasColumnName("providerUsername");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Account_userId_User_id_fk");
        });

        modelBuilder.Entity<Balance>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("Balance_pkey");

            entity.ToTable("Balance");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("userId");
            entity.Property(e => e.Available)
                .HasDefaultValue(0L)
                .HasColumnName("available");
            entity.Property(e => e.MaxMultiplier)
                .HasDefaultValue(0)
                .HasColumnName("maxMultiplier");
            entity.Property(e => e.MaxMultiplierGameId).HasColumnName("maxMultiplierGameId");
            entity.Property(e => e.MaxWin)
                .HasDefaultValue(0L)
                .HasColumnName("maxWin");
            entity.Property(e => e.MaxWinGameId).HasColumnName("maxWinGameId");
            entity.Property(e => e.TotalBet)
                .HasDefaultValue(0L)
                .HasColumnName("totalBet");
            entity.Property(e => e.TotalBetCount)
                .HasDefaultValue(0)
                .HasColumnName("totalBetCount");
            entity.Property(e => e.TotalLost)
                .HasDefaultValue(0L)
                .HasColumnName("totalLost");
            entity.Property(e => e.TotalWin)
                .HasDefaultValue(0L)
                .HasColumnName("totalWin");
            entity.Property(e => e.WageringRequired)
                .HasDefaultValue(0L)
                .HasColumnName("wageringRequired");

            entity.HasOne(d => d.MaxMultiplierGame).WithMany(p => p.BalanceMaxMultiplierGames)
                .HasForeignKey(d => d.MaxMultiplierGameId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Balance_maxMultiplierGameId_GameRecord_id_fk");

            entity.HasOne(d => d.MaxWinGame).WithMany(p => p.BalanceMaxWinGames)
                .HasForeignKey(d => d.MaxWinGameId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Balance_maxWinGameId_GameRecord_id_fk");

            entity.HasOne(d => d.User).WithOne(p => p.Balance)
                .HasForeignKey<Balance>(d => d.UserId)
                .HasConstraintName("Balance_userId_User_id_fk");
        });

        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Budget_pkey");

            entity.ToTable("Budget");

            entity.Property(e => e.Id)
                .HasDefaultValue(1)
                .HasColumnName("id");
            entity.Property(e => e.Available)
                .HasDefaultValue(10000000L)
                .HasColumnName("available");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.LastSyncAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("lastSyncAt");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ChatMessage_pkey");

            entity.ToTable("ChatMessage");

            entity.HasIndex(e => e.IsPinned, "ChatMessage_isPinned_index").HasFilter("(\"isPinned\" = true)");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("id");
            entity.Property(e => e.Attachments)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("attachments");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsPinned)
                .HasDefaultValue(false)
                .HasColumnName("isPinned");
            entity.Property(e => e.ProfileId).HasColumnName("profileId");
            entity.Property(e => e.SenderImage).HasColumnName("senderImage");
            entity.Property(e => e.SenderName).HasColumnName("senderName");
            entity.Property(e => e.SenderUsername).HasColumnName("senderUsername");
            entity.Property(e => e.Text).HasColumnName("text");
            entity.Property(e => e.TrackingId)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("trackingId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Profile).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.ProfileId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ChatMessage_profileId_Profile_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ChatMessage_userId_User_id_fk");
        });

        modelBuilder.Entity<Config>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Config_pkey");

            entity.ToTable("Config");

            entity.Property(e => e.Id)
                .HasDefaultValue(1)
                .HasColumnName("id");
            entity.Property(e => e.BackgroundJobsEnabled)
                .HasDefaultValue(true)
                .HasColumnName("backgroundJobsEnabled");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.MaintenanceEnabled)
                .HasDefaultValue(false)
                .HasColumnName("maintenanceEnabled");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Deposit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Deposit_pkey");

            entity.ToTable("Deposit");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.CurrencyAmount)
                .HasPrecision(20, 18)
                .HasColumnName("currencyAmount");
            entity.Property(e => e.GemAmount).HasColumnName("gemAmount");
            entity.Property(e => e.Payload)
                .HasColumnType("jsonb")
                .HasColumnName("payload");
            entity.Property(e => e.ProviderAmount).HasColumnName("providerAmount");
            entity.Property(e => e.ProviderTransactionId).HasColumnName("providerTransactionId");
            entity.Property(e => e.TransactionId).HasColumnName("transactionId");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Deposits)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Deposit_transactionId_Transaction_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Deposits)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Deposit_userId_User_id_fk");
        });

        modelBuilder.Entity<Models.Domain>(entity =>
        {
            entity.HasKey(e => e.Host).HasName("Domain_pkey");

            entity.ToTable("Domain");

            entity.Property(e => e.Host).HasColumnName("host");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
        });

        modelBuilder.Entity<GameRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GameRecord_pkey");

            entity.ToTable("GameRecord");

            entity.HasIndex(e => e.UserId, "GameRecord_userId_index");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("id");
            entity.Property(e => e.Bet).HasColumnName("bet");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.Multiplier).HasColumnName("multiplier");
            entity.Property(e => e.Payout).HasColumnName("payout");
            entity.Property(e => e.PreviewUserName).HasColumnName("previewUserName");
            entity.Property(e => e.Snapshot)
                .HasColumnType("jsonb")
                .HasColumnName("snapshot");
            entity.Property(e => e.TransactionId).HasColumnName("transactionId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Transaction).WithMany(p => p.GameRecords)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("GameRecord_transactionId_Transaction_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.GameRecords)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("GameRecord_userId_User_id_fk");
        });

        modelBuilder.Entity<GlobalTask>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("GlobalTask");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.Payout).HasColumnName("payout");
            entity.Property(e => e.Requirements)
                .HasColumnType("jsonb")
                .HasColumnName("requirements");
            entity.Property(e => e.WageringMultiplier).HasColumnName("wageringMultiplier");
        });

        modelBuilder.Entity<GlobalTaskStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("GlobalTaskStatus");

            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("GlobalTaskStatus_userId_User_id_fk");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Notification_pkey");

            entity.ToTable("Notification");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("id");
            entity.Property(e => e.AutoClose).HasColumnName("autoClose");
            entity.Property(e => e.AutoCloseMs).HasColumnName("autoCloseMs");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.ExpiresAt).HasColumnName("expiresAt");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.WithCloseButton).HasColumnName("withCloseButton");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Notification_userId_User_id_fk");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Profile_pkey");

            entity.ToTable("Profile");

            entity.HasIndex(e => e.UserId, "Profile_userId_unique").IsUnique();

            entity.HasIndex(e => e.Username, "Profile_username_unique").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.HasCustomName)
                .HasDefaultValue(false)
                .HasColumnName("hasCustomName");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Username).HasColumnName("username");

            entity.HasOne(d => d.User).WithOne(p => p.Profile)
                .HasForeignKey<Profile>(d => d.UserId)
                .HasConstraintName("Profile_userId_User_id_fk");
        });

        modelBuilder.Entity<Promocode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Promocode_pkey");

            entity.ToTable("Promocode");

            entity.HasIndex(e => e.Code, "Promocode_code_unique").IsUnique();

            entity.HasIndex(e => e.ExpiresAt, "Promocode_expiresAt_index");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("id");
            entity.Property(e => e.Bonus)
                .HasColumnType("jsonb")
                .HasColumnName("bonus");
            entity.Property(e => e.Campaign).HasColumnName("campaign");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy).HasColumnName("createdBy");
            entity.Property(e => e.ExpiresAt).HasColumnName("expiresAt");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.MaxUsages).HasColumnName("maxUsages");
            entity.Property(e => e.Usages).HasColumnName("usages");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.WageringMultiplier).HasColumnName("wageringMultiplier");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PromocodeCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Promocode_createdBy_User_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.PromocodeUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Promocode_userId_User_id_fk");
        });

        modelBuilder.Entity<PromocodeUsage>(entity =>
        {
            entity.HasKey(e => new { e.PromocodeId, e.UserId }).HasName("PromocodeUsage_promocodeId_userId_pk");

            entity.ToTable("PromocodeUsage");

            entity.Property(e => e.PromocodeId).HasColumnName("promocodeId");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.ExpiresAt).HasColumnName("expiresAt");

            entity.HasOne(d => d.Promocode).WithMany(p => p.PromocodeUsages)
                .HasForeignKey(d => d.PromocodeId)
                .HasConstraintName("PromocodeUsage_promocodeId_Promocode_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.PromocodeUsages)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("PromocodeUsage_userId_User_id_fk");
        });

        modelBuilder.Entity<ReferralCampaign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ReferralCampaign_pkey");

            entity.ToTable("ReferralCampaign");

            entity.HasIndex(e => e.Code, "ReferralCampaign_code_unique").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.ReferrerId).HasColumnName("referrerId");
            entity.Property(e => e.TotalSignups)
                .HasDefaultValue(0)
                .HasColumnName("totalSignups");
            entity.Property(e => e.TotalVisits)
                .HasDefaultValue(0)
                .HasColumnName("totalVisits");

            entity.HasOne(d => d.Referrer).WithMany(p => p.ReferralCampaigns)
                .HasForeignKey(d => d.ReferrerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ReferralCampaign_referrerId_User_id_fk");
        });

        modelBuilder.Entity<ReferrerBalance>(entity =>
        {
            entity.HasKey(e => e.ReferrerId).HasName("ReferrerBalance_pkey");

            entity.ToTable("ReferrerBalance");

            entity.Property(e => e.ReferrerId)
                .ValueGeneratedNever()
                .HasColumnName("referrerId");
            entity.Property(e => e.Available)
                .HasDefaultValue(0L)
                .HasColumnName("available");

            entity.HasOne(d => d.Referrer).WithOne(p => p.ReferrerBalance)
                .HasForeignKey<ReferrerBalance>(d => d.ReferrerId)
                .HasConstraintName("ReferrerBalance_referrerId_User_id_fk");
        });

        modelBuilder.Entity<ReferrerPayout>(entity =>
        {
            entity.HasKey(e => e.ReferrerId).HasName("ReferrerPayout_pkey");

            entity.ToTable("ReferrerPayout");

            entity.Property(e => e.ReferrerId)
                .ValueGeneratedNever()
                .HasColumnName("referrerId");
            entity.Property(e => e.LastPayoutAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lastPayoutAt");
            entity.Property(e => e.NextPayoutAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("nextPayoutAt");

            entity.HasOne(d => d.Referrer).WithOne(p => p.ReferrerPayout)
                .HasForeignKey<ReferrerPayout>(d => d.ReferrerId)
                .HasConstraintName("ReferrerPayout_referrerId_User_id_fk");
        });

        modelBuilder.Entity<ReferrerSetting>(entity =>
        {
            entity.HasKey(e => e.ReferrerId).HasName("ReferrerSettings_pkey");

            entity.Property(e => e.ReferrerId)
                .ValueGeneratedNever()
                .HasColumnName("referrerId");
            entity.Property(e => e.RevShare)
                .HasDefaultValue(10)
                .HasColumnName("revShare");

            entity.HasOne(d => d.Referrer).WithOne(p => p.ReferrerSetting)
                .HasForeignKey<ReferrerSetting>(d => d.ReferrerId)
                .HasConstraintName("ReferrerSettings_referrerId_User_id_fk");
        });

        modelBuilder.Entity<ReferrerTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ReferrerTransaction_pkey");

            entity.ToTable("ReferrerTransaction");

            entity.HasIndex(e => e.CreatedAt, "ReferrerTransaction_createdAt_index");

            entity.HasIndex(e => e.ReferrerId, "ReferrerTransaction_referrerId_index");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsProcessed)
                .HasDefaultValue(false)
                .HasColumnName("isProcessed");
            entity.Property(e => e.ReferralCampaignId).HasColumnName("referralCampaignId");
            entity.Property(e => e.ReferralId).HasColumnName("referralId");
            entity.Property(e => e.ReferrerId).HasColumnName("referrerId");

            entity.HasOne(d => d.ReferralCampaign).WithMany(p => p.ReferrerTransactions)
                .HasForeignKey(d => d.ReferralCampaignId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("ReferrerTransaction_referralCampaignId_ReferralCampaign_id_fk");

            entity.HasOne(d => d.Referral).WithMany(p => p.ReferrerTransactionReferrals)
                .HasForeignKey(d => d.ReferralId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("ReferrerTransaction_referralId_User_id_fk");

            entity.HasOne(d => d.Referrer).WithMany(p => p.ReferrerTransactionReferrers)
                .HasForeignKey(d => d.ReferrerId)
                .HasConstraintName("ReferrerTransaction_referrerId_User_id_fk");
        });

        modelBuilder.Entity<ReferrerWithdrawal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ReferrerWithdrawal_pkey");

            entity.ToTable("ReferrerWithdrawal");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.ReferrerId).HasColumnName("referrerId");

            entity.HasOne(d => d.Referrer).WithMany(p => p.ReferrerWithdrawals)
                .HasForeignKey(d => d.ReferrerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ReferrerWithdrawal_referrerId_User_id_fk");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Session_pkey");

            entity.ToTable("Session");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("id");
            entity.Property(e => e.ExpiresAt).HasColumnName("expiresAt");
            entity.Property(e => e.ReferralCampaignId).HasColumnName("referralCampaignId");
            entity.Property(e => e.ReferrerId).HasColumnName("referrerId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.ReferralCampaign).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.ReferralCampaignId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Session_referralCampaignId_ReferralCampaign_id_fk");

            entity.HasOne(d => d.Referrer).WithMany(p => p.SessionReferrers)
                .HasForeignKey(d => d.ReferrerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Session_referrerId_User_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.SessionUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Session_userId_User_id_fk");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Transaction_pkey");

            entity.ToTable("Transaction");

            entity.HasIndex(e => e.CreatedAt, "Transaction_createdAt_index");

            entity.HasIndex(e => e.UserId, "Transaction_userId_index");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.GameRecordId).HasColumnName("gameRecordId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Transaction_userId_User_id_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.ToTable("User");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.ProfileId).HasColumnName("profileId");
            entity.Property(e => e.ReferralCampaignId).HasColumnName("referralCampaignId");
            entity.Property(e => e.ReferrerId).HasColumnName("referrerId");
            entity.Property(e => e.SecurityId).HasColumnName("securityId");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updatedAt");
            entity.Property(e => e.Virtual)
                .HasDefaultValue(false)
                .HasColumnName("virtual");

            entity.HasOne(d => d.Referrer).WithMany(p => p.InverseReferrer)
                .HasForeignKey(d => d.ReferrerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("User_referrerId_User_id_fk");
        });

        modelBuilder.Entity<UserSecurity>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("UserSecurity_pkey");

            entity.ToTable("UserSecurity");

            entity.HasIndex(e => e.LastIp, "UserSecurity_lastIP_index");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("userId");
            entity.Property(e => e.AddressBnb).HasColumnName("addressBnb");
            entity.Property(e => e.AddressBnbScore)
                .HasDefaultValue(0)
                .HasColumnName("addressBnbScore");
            entity.Property(e => e.AddressBtc).HasColumnName("addressBtc");
            entity.Property(e => e.AddressBtcScore)
                .HasDefaultValue(0)
                .HasColumnName("addressBtcScore");
            entity.Property(e => e.AddressDoge).HasColumnName("addressDoge");
            entity.Property(e => e.AddressDogeScore)
                .HasDefaultValue(0)
                .HasColumnName("addressDogeScore");
            entity.Property(e => e.AddressEth).HasColumnName("addressEth");
            entity.Property(e => e.AddressEthScore)
                .HasDefaultValue(0)
                .HasColumnName("addressEthScore");
            entity.Property(e => e.AddressLtc).HasColumnName("addressLtc");
            entity.Property(e => e.AddressLtcScore)
                .HasDefaultValue(0)
                .HasColumnName("addressLtcScore");
            entity.Property(e => e.AddressSol).HasColumnName("addressSol");
            entity.Property(e => e.AddressSolScore)
                .HasDefaultValue(0)
                .HasColumnName("addressSolScore");
            entity.Property(e => e.AddressTon).HasColumnName("addressTon");
            entity.Property(e => e.AddressTonScore)
                .HasDefaultValue(0)
                .HasColumnName("addressTonScore");
            entity.Property(e => e.AddressTrx).HasColumnName("addressTrx");
            entity.Property(e => e.AddressTrxScore)
                .HasDefaultValue(0)
                .HasColumnName("addressTrxScore");
            entity.Property(e => e.AddressUsdc).HasColumnName("addressUsdc");
            entity.Property(e => e.AddressUsdcScore)
                .HasDefaultValue(0)
                .HasColumnName("addressUsdcScore");
            entity.Property(e => e.AddressUsdt).HasColumnName("addressUsdt");
            entity.Property(e => e.AddressUsdtScore)
                .HasDefaultValue(0)
                .HasColumnName("addressUsdtScore");
            entity.Property(e => e.AddressXmr).HasColumnName("addressXmr");
            entity.Property(e => e.AddressXmrScore)
                .HasDefaultValue(0)
                .HasColumnName("addressXmrScore");
            entity.Property(e => e.IpScore)
                .HasDefaultValue(0)
                .HasColumnName("ipScore");
            entity.Property(e => e.LastIp).HasColumnName("lastIP");
            entity.Property(e => e.Whitelisted)
                .HasDefaultValue(false)
                .HasColumnName("whitelisted");

            entity.HasOne(d => d.User).WithOne(p => p.UserSecurity)
                .HasForeignKey<UserSecurity>(d => d.UserId)
                .HasConstraintName("UserSecurity_userId_User_id_fk");
        });

        modelBuilder.Entity<UserStat>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("UserStats_pkey");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("userId");
            entity.Property(e => e.DepositCount)
                .HasDefaultValue(0)
                .HasColumnName("depositCount");
            entity.Property(e => e.WithdrawCount)
                .HasDefaultValue(0)
                .HasColumnName("withdrawCount");

            entity.HasOne(d => d.User).WithOne(p => p.UserStat)
                .HasForeignKey<UserStat>(d => d.UserId)
                .HasConstraintName("UserStats_userId_User_id_fk");
        });

        modelBuilder.Entity<Withdrawal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Withdrawal_pkey");

            entity.ToTable("Withdrawal");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v7()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.CurrencyAmount)
                .HasPrecision(20, 18)
                .HasColumnName("currencyAmount");
            entity.Property(e => e.GemAmount).HasColumnName("gemAmount");
            entity.Property(e => e.ProviderAmount).HasColumnName("providerAmount");
            entity.Property(e => e.ProviderTransactionId).HasColumnName("providerTransactionId");
            entity.Property(e => e.TransactionId).HasColumnName("transactionId");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Withdrawals)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Withdrawal_transactionId_Transaction_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Withdrawals)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Withdrawal_userId_User_id_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
