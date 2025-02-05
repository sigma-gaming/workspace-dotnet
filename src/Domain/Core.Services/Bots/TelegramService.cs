using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Core.Services.Bots;

public class TelegramService(
    TelegramOptions options)
{
    private TelegramBotClient Client { get; } = new(options.BotToken);

    public async Task MessageUserAsync(long userId, string message)
    {
        await Client.SendMessage(new ChatId(userId), message, ParseMode.Markdown);
    }

    private bool IsSubscribedStatus(ChatMemberStatus status)
    {
        return status is ChatMemberStatus.Creator or ChatMemberStatus.Administrator or ChatMemberStatus.Creator
            or ChatMemberStatus.Restricted;
    }

    public async Task<bool> CheckSubscriptionAsync(long userId, ChatId chatId)
    {
        var member = await Client.GetChatMember(chatId, userId);
        return IsSubscribedStatus(member.Status);
    }
}