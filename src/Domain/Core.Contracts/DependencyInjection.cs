using Core.Builder;
using Core.Contracts.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Contracts;

public static class DependencyInjection
{
    public static CoreBuilder AddMappers(
        this CoreBuilder builder)
    {
        builder.Services.AddSingleton<UserMapper>();
        builder.Services.AddSingleton<UserStatsMapper>();
        builder.Services.AddSingleton<AccountMapper>();
        builder.Services.AddSingleton<ProfileMapper>();
        builder.Services.AddSingleton<SessionMapper>();
        builder.Services.AddSingleton<BudgetMapper>();
        builder.Services.AddSingleton<BalanceMapper>();
        builder.Services.AddSingleton<TransactionMapper>();
        builder.Services.AddSingleton<GameRecordMapper>();
        builder.Services.AddSingleton<ReferralCampaignMapper>();
        builder.Services.AddSingleton<ReferrerSettingsMapper>();
        builder.Services.AddSingleton<ReferrerBalanceMapper>();
        builder.Services.AddSingleton<ReferrerPayoutMapper>();
        builder.Services.AddSingleton<ReferrerTransactionMapper>();
        builder.Services.AddSingleton<ReferrerWithdrawalMapper>();
        builder.Services.AddSingleton<GlobalTaskMapper>();
        builder.Services.AddSingleton<PromocodeMapper>();
        builder.Services.AddSingleton<NotificationMapper>();
        builder.Services.AddSingleton<ChatMessageMapper>();
        builder.Services.AddSingleton<DomainMapper>();

        return builder;
    }
}