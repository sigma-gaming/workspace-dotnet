using Core.Contracts.Data;
using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Db.Context;
using Core.Services.Bots;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Promotions.TaskCheckers;

public class TelegramGroupSubscribeChecker(
    CoreDbContext db,
    TelegramService telegramService) : IGlobalTaskChecker
{
    public async Task<GlobalTaskCheckResult> CheckAsync(Guid userId, GlobalTask task)
    {
        if (task.Requirements is not GlobalTaskRequirementsTelegramGroupSubscribe requirements)
            return new GlobalTaskCheckResult(
                false,
                "Не удалось проверить задание. Попробуйте ещё раз или напишите в поддержку");

        var account = await db.Accounts.FirstOrDefaultAsync(a =>
            a.Provider == AccountProvider.Telegram &&
            a.UserId == userId);

        if (account == null)
            return new GlobalTaskCheckResult(
                false,
                "Для начала нужно привязать аккаунт Telegram");

        if (!long.TryParse(account.ProviderUserId, out var telegramUserId)) return new GlobalTaskCheckResult(false);

        var subscribed = await telegramService.CheckSubscriptionAsync(
            telegramUserId,
            requirements.GroupId);

        return new GlobalTaskCheckResult(subscribed);
    }
}