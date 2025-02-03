using Core.Contracts.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Contracts;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreMappers(
        this IServiceCollection services)
    {
        services.AddSingleton<UserMapper>();
        services.AddSingleton<AccountMapper>();
        services.AddSingleton<ProfileMapper>();
        services.AddSingleton<SessionMapper>();
        services.AddSingleton<BudgetMapper>();
        services.AddSingleton<BalanceMapper>();
        services.AddSingleton<TransactionMapper>();
        services.AddSingleton<GameRecordMapper>();
        services.AddSingleton<ReferralCampaignMapper>();
        services.AddSingleton<ReferrerSettingsMapper>();
        services.AddSingleton<ReferrerBalanceMapper>();
        services.AddSingleton<ReferrerPayoutMapper>();
        services.AddSingleton<ReferrerTransactionMapper>();
        services.AddSingleton<ReferrerWithdrawalMapper>();
        services.AddSingleton<GlobalTaskMapper>();

        return services;
    }
}