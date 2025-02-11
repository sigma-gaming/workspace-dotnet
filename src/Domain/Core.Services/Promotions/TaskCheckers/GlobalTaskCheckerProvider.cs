using Core.Contracts.Enums;

namespace Core.Services.Promotions.TaskCheckers;

public class GlobalTaskCheckerProvider(
    TelegramGroupSubscribeChecker telegramGroupSubscribeChecker,
    VkGroupSubscribeChecker vkGroupSubscribeChecker,
    VkPinnedRepostChecker vkPinnedRepostChecker)
{
    public IGlobalTaskChecker? GetChecker(GlobalTaskKey taskKey)
    {
        return taskKey switch
        {
            GlobalTaskKey.TelegramGroupSubscribe => telegramGroupSubscribeChecker,
            GlobalTaskKey.VkGroupSubscribe => vkGroupSubscribeChecker,
            GlobalTaskKey.VkPinnedRepost => vkPinnedRepostChecker,
            _ => null,
        };
    }
}