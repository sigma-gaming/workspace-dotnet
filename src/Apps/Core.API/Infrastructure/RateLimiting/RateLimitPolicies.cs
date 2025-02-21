namespace Core.API.Infrastructure.RateLimiting;

public static class RateLimitPolicies
{
    public static class Game
    {
        public const string Play = "game-play";
    }

    public static class Chat
    {
        public const string Message = "chat-message";
    }

    public static class Task
    {
        public const string Action = "task-action";
    }

    public static class Promocode
    {
        public const string Apply = "promocode-apply";
    }

    public static class Affiliate
    {
        public const string Withdraw = "affiliate-withdraw";
    }

    public static class Profile
    {
        public const string Update = "profile-update";
    }
}