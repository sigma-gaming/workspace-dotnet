using System.Security.Cryptography;
using System.Text;
using Core.Services.Telegram;
using Microsoft.Extensions.Options;

namespace LetsAuth.API.Integrations;

public sealed record TelegramAuthData
{
    public required long Id { get; init; }
    public required string FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Username { get; init; }
    public string? PhotoUrl { get; init; }
    public required long AuthDate { get; init; }
    public required string Hash { get; init; }
}

public class TelegramIntegration(ILogger<TelegramIntegration> logger, IOptions<TelegramOptions> options)
{
    public bool VerifyAuthHash(TelegramAuthData authData)
    {
        try
        {
            var hashToCompareWith = authData.Hash;

            // Build the data check string by sorting parameters
            var dataCheckParams = new Dictionary<string, string>
            {
                ["id"] = authData.Id.ToString(),
                ["first_name"] = authData.FirstName,
                ["auth_date"] = authData.AuthDate.ToString(),
            };

            // Add optional parameters if present
            if (!string.IsNullOrEmpty(authData.LastName))
                dataCheckParams["last_name"] = authData.LastName;

            if (!string.IsNullOrEmpty(authData.Username))
                dataCheckParams["username"] = authData.Username;

            if (!string.IsNullOrEmpty(authData.PhotoUrl))
                dataCheckParams["photo_url"] = authData.PhotoUrl;

            // Sort parameters and create the check string
            var dataCheckString = string.Join("\n",
                dataCheckParams.OrderBy(kv => kv.Key)
                    .Select(kv => $"{kv.Key}={kv.Value}"));

            // Create SHA-256 hash of the bot token
            var secretKey = SHA256.HashData(Encoding.UTF8.GetBytes(options.Value.BotToken));

            // Create HMAC with the secret key
            using var hmac = new HMACSHA256(secretKey);
            var calculatedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));

            // Convert to hex string for comparison
            var calculatedHashHex = Convert.ToHexStringLower(calculatedHash);

            return calculatedHashHex == hashToCompareWith;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error verifying Telegram auth hash");
            return false;
        }
    }
}