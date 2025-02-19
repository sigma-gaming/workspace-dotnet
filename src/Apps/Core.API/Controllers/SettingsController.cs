using System.ComponentModel.DataAnnotations;
using Core.Contracts.Compounds;
using Core.Contracts.Enums;
using Core.Db.Context;
using Core.Services.Auth;
using Core.Services.Caching;
using Core.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.API.Controllers;

public sealed record UpdateProfilePayload
{
    [Length(3, 20, ErrorMessage = "Допустимая длина - от 3 до 20 символов")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$",
        ErrorMessage = "Допустимы только латинские буквы, цифры и нижнее подчеркивание")]
    public string? Username { get; init; }

    [Length(3, 32, ErrorMessage = "Допустимая длина - от 3 до 32 символов")]
    [RegularExpression(@"^[!#$€£%&'""'()*+-./:;,=<>?@\\^|А-Яа-яёЁA-Za-z0-9 ]+$",
        ErrorMessage = "Содержатся недопустимые символы")]
    public string? Name { get; init; }

    public AccountProvider Provider { get; init; }
}

public sealed record ProfileUpdates
{
    public string? Username { get; init; }
    public AccountProvider? UsedProvider { get; init; }
    public string? Image { get; set; }
    public string? Name { get; set; }
    public bool? HasCustomName { get; set; }
}

[Authorize]
[ApiController]
public class SettingsController(
    CoreDbContext db,
    CoreCacheRegistry cache,
    ProfileService profileService) : ControllerBase
{
    [ProducesResponseType<UserDetails>(StatusCodes.Status200OK, "application/json")]
    [HttpPut("/settings/profile")]
    public async Task<Results<Ok<UserDetails>, ValidationProblem>> UpdateProfile(
        [FromBody] UpdateProfilePayload payload)
    {
        var userId = HttpContext.GetUserId();
        var isAdmin = HttpContext.User.IsInRole(nameof(UserRole.Admin));

        if (payload.Username is not null)
        {
            if (!isAdmin && ProfileService.ReservedUsernames.Contains(payload.Username))
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(UpdateProfilePayload.Username)] = ["Недопустимый никнейм"],
                });

            var currentProfile = await db.Profiles
                .FirstOrDefaultAsync(p => p.Username == payload.Username);

            if (currentProfile != null && currentProfile.UserId != userId)
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(UpdateProfilePayload.Username)] = ["Пользователь с таким никнеймом уже существует"],
                });
        }

        var accounts = await db.Accounts
            .Where(a => a.UserId == userId)
            .ToListAsync();

        var selectedProvider = accounts.FirstOrDefault(a => a.Provider == payload.Provider);

        if (selectedProvider == null)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(UpdateProfilePayload.Provider)] = ["Не привязан необходимый аккаунт"],
            });

        var updates = new ProfileUpdates
        {
            Username = payload.Username,
            UsedProvider = payload.Provider,
            Image = selectedProvider.ProviderUserImage,
        };

        var nameFromProvider = ProfileService.GetUserFullName(
            selectedProvider.ProviderUserFirstName,
            selectedProvider.ProviderUserLastName);

        if (payload.Name is not null)
        {
            updates.Name = payload.Name;
            updates.HasCustomName = payload.Name != nameFromProvider;
        }
        else
        {
            updates.Name = nameFromProvider;
            updates.HasCustomName = false;
        }

        await db.Profiles
            .Where(p => p.UserId == userId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.Username, updates.Username)
                .SetProperty(p => p.UsedProvider, updates.UsedProvider)
                .SetProperty(p => p.Image, updates.Image)
                .SetProperty(p => p.Name, updates.Name)
                .SetProperty(p => p.HasCustomName, updates.HasCustomName)
            );

        await cache.ExecuteIfConnectedAsync(async () => { await cache.UserDetails.DeleteAsync(userId); });

        var details = await profileService.GetUserDetailsAsync(userId);
        return TypedResults.Ok(details);
    }
}