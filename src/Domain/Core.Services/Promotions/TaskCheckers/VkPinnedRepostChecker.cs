using Core.Contracts.Data;
using Core.Contracts.Enums;
using Core.Db.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Promotions.TaskCheckers;

public class VkPinnedRepostChecker(
    CoreDbContext db
    // VkService vkService
) : IGlobalTaskChecker
{
    public async Task<GlobalTaskCheckResult> CheckAsync(Guid userId, GlobalTask task)
    {
        if (task.Requirements.Type != GlobalTaskKey.VkPinnedRepost)
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

        // if (!long.TryParse(account.ProviderUserId, out var vkUserId))
        //     return new GlobalTaskCheckResult(false);

        // TODO: Implement VK checker
        return new GlobalTaskCheckResult(false, "Попробуйте позже");

        // var repostStatus = await vkService.GetRepostStatusAsync(
        //     vkUserId,
        //     task.Requirements.GroupId,
        //     task.Requirements.PostId);
        //
        // return repostStatus switch
        // {
        //     RepostStatus.Reposted => new GlobalTaskCheckResult(true),
        //     RepostStatus.NotFound => new GlobalTaskCheckResult(false),
        //     RepostStatus.WallNotAvailable => new GlobalTaskCheckResult(
        //         false,
        //         "Ваш профиль VK и/или стена закрыты"),
        //     RepostStatus.ProfileDeleted => new GlobalTaskCheckResult(
        //         false,
        //         "Ваша страница VK удалена или заморожена"),
        //     RepostStatus.TooManyRequests => new GlobalTaskCheckResult(
        //         false,
        //         "Сейчас мы не можем проверить задание, попробуйте позже"),
        //     _ => new GlobalTaskCheckResult(
        //         false,
        //         "Не удалось проверить задание. Попробуйте ещё раз или напишите в поддержку")
        // };
    }
}