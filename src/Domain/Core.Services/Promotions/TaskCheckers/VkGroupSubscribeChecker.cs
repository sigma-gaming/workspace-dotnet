using Core.Contracts.Data;
using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Db.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Promotions.TaskCheckers;

public class VkGroupSubscribeChecker(
    CoreDbContext db
    // VkService vkService
) : IGlobalTaskChecker
{
    public async Task<GlobalTaskCheckResult> CheckAsync(Guid userId, GlobalTask task)
    {
        if (task.Requirements is not GlobalTaskRequirementsVkGroupSubscribe requirements)
            return new GlobalTaskCheckResult(
                false,
                "Не удалось проверить задание. Попробуйте ещё раз или напишите в поддержку");

        var account = await db.Accounts.FirstOrDefaultAsync(a =>
            a.Provider == AccountProvider.Vk &&
            a.UserId == userId);

        if (account == null)
            return new GlobalTaskCheckResult(
                false,
                "Для начала нужно привязать аккаунт VK");

        if (!long.TryParse(account.ProviderUserId, out var vkUserId)) return new GlobalTaskCheckResult(false);

        // TODO: Implement VK checker
        return new GlobalTaskCheckResult(false, "Попробуйте позже");

        // var subscribed = await vkService.CheckSubscriptionAsync(
        //     vkUserId,
        //     requirements.GroupId);
        //
        // return new GlobalTaskCheckResult(subscribed);
    }
}