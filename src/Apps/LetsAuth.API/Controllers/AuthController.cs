using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Common.Api.Responses;
using Core.Contracts.Enums;
using Core.Services.Auth;
using Core.Services.Caching;
using Core.Services.User;
using LetsAuth.API.Infrastructure.RateLimiting;
using LetsAuth.API.Integrations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LetsAuth.API.Controllers;

public sealed record AuthenticatePayload
{
    public required string App { get; init; }
    public required AuthenticationAction Action { get; init; }
    public required AuthenticationIntegration Integration { get; init; }
    public required string Payload { get; init; }
}

public sealed record AuthenticateResponse
{
    public required AuthenticateOutcome Outcome { get; init; }
    public string? Code { get; init; }
}

[ApiController]
public class AuthController(
    SessionManager sessionManager,
    SessionService sessionService,
    AuthService authService,
    TelegramIntegration telegramIntegration,
    VkIntegration vkIntegration,
    FraudService fraudService,
    CoreCacheRegistry cache,
    ILogger<AuthController> logger) : ControllerBase
{
    private const string LATIN_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private static readonly JsonSerializerOptions VkJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    private static readonly JsonSerializerOptions TelegramJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    private static readonly string CodeAlphabet = $"{LATIN_ALPHABET}{LATIN_ALPHABET.ToLower()}0123456789";

    private string? GetAppCookie(string app, string cookieName)
    {
        return Request.Cookies[$"{app}_{cookieName}"];
    }

    private static string GetProviderLabel(AuthenticationIntegration integration)
    {
        return integration switch
        {
            AuthenticationIntegration.Vk => "ВКонтакте",
            AuthenticationIntegration.Telegram => "Telegram",
            _ => throw new Exception("Unsupported integration"),
        };
    }

    private static string GenerateCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var chars = new char[12];

        for (var i = 0; i < chars.Length; i++)
        {
            var randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            var value = BitConverter.ToInt32(randomBytes, 0);
            chars[i] = CodeAlphabet[Math.Abs(value) % CodeAlphabet.Length];
        }

        return new string(chars);
    }

    private Task<Core.Services.Auth.AuthenticatePayload> HandleTelegramAuthentication(string payload, Guid? userId,
        string? referralCampaignCode)
    {
        TelegramAuthData? authData;

        try
        {
            var normalized = TelegramIntegration.NormalizeTelegramPayload(payload);
            var jsonPayload = Encoding.UTF8.GetString(Convert.FromBase64String(normalized));
            authData = JsonSerializer.Deserialize<TelegramAuthData>(jsonPayload, TelegramJsonOptions);

            if (authData is null) throw new Exception("Invalid Telegram payload");
        }
        catch (Exception ex) when (ex is FormatException or JsonException)
        {
            logger.LogError(ex, "Invalid Telegram payload format");
            throw new Exception("Invalid Telegram payload format");
        }

        if (!telegramIntegration.VerifyAuthHash(authData))
            throw new Exception("Invalid Telegram authentication data");

        var authTimestamp = authData.AuthDate * 1000;
        var validUntil = DateTimeOffset.FromUnixTimeMilliseconds(authTimestamp)
            .AddHours(1);

        // TODO: Add production check
        if (DateTimeOffset.UtcNow > validUntil)
            throw new Exception("Telegram authentication data is expired");

        return Task.FromResult(new Core.Services.Auth.AuthenticatePayload
        {
            UserId = userId,
            Provider = AccountProvider.Telegram,
            ProviderUserId = authData.Id.ToString(),
            ProviderUsername = authData.Username,
            ProviderUserFirstName = authData.FirstName,
            ProviderUserLastName = authData.LastName,
            ProviderUserImage = authData.PhotoUrl,
            ReferralCampaignCode = referralCampaignCode,
        });
    }

    private async Task<Core.Services.Auth.AuthenticatePayload> HandleVkAuthentication(string payload, Guid? userId,
        string? referralCampaignCode)
    {
        VkAuthData? authData;

        try
        {
            authData = JsonSerializer.Deserialize<VkAuthData>(payload, VkJsonOptions);

            if (authData is null || authData.Type != "silent_token") throw new Exception("Invalid VK payload");
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Invalid VK payload");
            throw;
        }

        var tokenExchangeResult = await vkIntegration.ExchangeSilentAuthToken(
            authData.Token,
            authData.Uuid);

        var vkProfile = await vkIntegration.GetProfile(tokenExchangeResult.AccessToken);

        return new Core.Services.Auth.AuthenticatePayload
        {
            UserId = userId,
            Provider = AccountProvider.Vk,
            ProviderUserId = vkProfile.Id.ToString(),
            ProviderUsername = vkProfile.ScreenName,
            ProviderUserFirstName = vkProfile.FirstName,
            ProviderUserLastName = vkProfile.LastName,
            ProviderUserImage = vkProfile.Photo200,
            ReferralCampaignCode = referralCampaignCode,
        };
    }

    [EnableRateLimiting(RateLimitPolicies.Authenticate)]
    [ProducesResponseType<AuthenticateResponse>(StatusCodes.Status200OK, "application/json")]
    [HttpPost("/authenticate")]
    public async Task<Results<Ok<AuthenticateResponse>, BadRequest<ErrorResponse>>> Authenticate(
        [FromBody] AuthenticatePayload payload)
    {
        if (payload.App != "sigma")
            return TypedResults.BadRequest(new ErrorResponse("Приложение не найдено"));

        if (!cache.IsReady())
            return TypedResults.BadRequest(new ErrorResponse("Сервис временно недоступен, повторите попытку позже"));

        Guid? userId = null;

        if (payload.Action == AuthenticationAction.Connect)
        {
            userId = HttpContext.TryGetUserId();

            if (userId is null)
                return TypedResults.Ok(new AuthenticateResponse
                {
                    Outcome = AuthenticateOutcome.NotAuthenticated,
                });
        }

        var referralCampaignCode = GetAppCookie(payload.App, "referralCampaign");

        var authenticatePayload = payload.Integration switch
        {
            AuthenticationIntegration.Telegram => await HandleTelegramAuthentication(payload.Payload, userId,
                referralCampaignCode),
            AuthenticationIntegration.Vk => await HandleVkAuthentication(payload.Payload, userId, referralCampaignCode),
            _ => throw new Exception("Unsupported integration"),
        };

        var result = await authService.AuthenticateAsync(authenticatePayload);
        var label = GetProviderLabel(payload.Integration);

        if (result.Outcome == AuthOutcome.ConnectedToAnotherUser)
            return TypedResults.BadRequest(new ErrorResponse($"Аккаунт {label} уже привязан к другому пользователю"));

        if (result.Outcome == AuthOutcome.AnotherAccountConnected)
            return TypedResults.BadRequest(new ErrorResponse($"Другой аккаунт {label} уже привязан к вашему профилю"));

        if (result.Outcome == AuthOutcome.Connected)
            return TypedResults.Ok(new AuthenticateResponse
            {
                Outcome = AuthenticateOutcome.Connected,
            });

        var session = await sessionService.CreateSession(new CreateSessionPayload
        {
            UserId = result.User!.Id,
            ReferrerId = result.User.ReferrerId,
            ReferralCampaignId = result.User.ReferralCampaignId,
            Provider = payload.Integration switch
            {
                AuthenticationIntegration.Vk => AccountProvider.Vk,
                AuthenticationIntegration.Telegram => AccountProvider.Telegram,
                _ => throw new Exception("Unsupported integration"),
            },
        });

        var code = GenerateCode();

        await cache.SessionCodeToSessionId.SetAsync(code, session.Id.ToString());
        sessionManager.AttachSession(HttpContext, new SessionInfo(session.Id, session.ExpiresAt));

        // TODO: Check IP
        await fraudService.ActualizeRiskAsync(result.User!.Id);

        return TypedResults.Ok(new AuthenticateResponse
        {
            Outcome = result.Outcome switch
            {
                AuthOutcome.SignedIn => AuthenticateOutcome.SignedIn,
                AuthOutcome.SignedUp => AuthenticateOutcome.SignedUp,
                _ => throw new Exception("Unexpected auth outcome"),
            },
            Code = code,
        });
    }
}