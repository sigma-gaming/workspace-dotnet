using System.ComponentModel.DataAnnotations;
using Common.Api.Responses;
using Common.Db.Extensions;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Db.Context;
using Core.Services.Auth;
using Core.Services.Promotions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Control.API.Controllers;

public sealed record CreatePromocodePayload
{
    [Required]
    [MinLength(1, ErrorMessage = "Не может быть пустым")]
    public required string Campaign { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Не может быть меньше 1")]
    public required int Count { get; init; }

    public string? Code { get; init; }

    [Required] public required DateTime ExpiresAt { get; init; }

    [Required] public required PromocodeBonusType BonusType { get; init; }

    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "Не может быть меньше 1 гема")]
    public required long Payout { get; init; }

    [Required] public required int WageringMultiplier { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Не может быть меньше 1")]
    public required int MaxUsages { get; init; }

    public Guid? UserId { get; init; }

    [Required] public required bool IsActive { get; init; }
}

public sealed record CreatePromocodeResponse
{
    public required string[] Codes { get; init; }
}

public sealed record GeneratePromocodeResponse
{
    public required string Promocode { get; init; }
}

[ApiController]
public class PromocodesController(CoreDbContext db) : ControllerBase
{
    [ProducesResponseType<GeneratePromocodeResponse>(StatusCodes.Status200OK, "application/json")]
    [HttpGet("/promocodes/random")]
    public Ok<GeneratePromocodeResponse> Generate()
    {
        var promocode = PromocodeService.GenerateOne();
        return TypedResults.Ok(new GeneratePromocodeResponse
        {
            Promocode = promocode,
        });
    }

    [AuthorizeRoles(UserRole.Admin)]
    [ProducesResponseType<CreatePromocodeResponse>(StatusCodes.Status200OK, "application/json")]
    [HttpPost("/promocodes/create")]
    public async Task<Results<Ok<CreatePromocodeResponse>, ValidationProblem, BadRequest<ErrorResponse>>> Create(
        [FromBody] CreatePromocodePayload payload)
    {
        if (payload.Count == 1 && string.IsNullOrEmpty(payload.Code))
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(CreatePromocodePayload.Code)] = ["Не может быть пустым"],
            });

        if (payload.UserId is not null && payload.MaxUsages > 1)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(CreatePromocodePayload.MaxUsages)] = ["Персональный промокод должен быть одноразовым"],
            });

        if (DateTime.UtcNow >= payload.ExpiresAt)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(CreatePromocodePayload.ExpiresAt)] = ["Дата истечения уже наступила"],
            });

        var codes = new List<string>();

        if (payload.Count == 1)
            codes.Add(payload.Code!);
        else
            codes.AddRange(PromocodeService.GenerateMany(payload.Count));

        var existingCount = await db.Promocodes
            .Where(p => codes.Contains(p.Code))
            .CountAsync();

        if (existingCount > 0)
        {
            if (payload.Count == 1)
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(CreatePromocodePayload.Code)] = ["Промокод уже существует"],
                });

            return TypedResults.BadRequest(
                new ErrorResponse("Один или несколько промокодов уже существуют. Попробуйте еще раз"));
        }

        PromocodeBonus bonus;

        if (payload.BonusType == PromocodeBonusType.Payout)
            bonus = new PromocodeBonusPayout
            {
                Payout = payload.Payout,
            };
        else
            // For simplicity just assuming Payout type is required
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(CreatePromocodePayload.BonusType)] = ["Неподдерживаемый тип бонуса"],
            });

        var userId = HttpContext.GetUserId();

        await db.ExecuteInTransactionAsync(async () =>
        {
            foreach (var code in codes)
                await db.Promocodes.AddAsync(new PromocodeEntity
                {
                    Campaign = payload.Campaign,
                    Code = code,
                    BonusType = payload.BonusType,
                    Bonus = bonus,
                    WageringMultiplier = payload.WageringMultiplier,
                    Usages = 0,
                    MaxUsages = payload.MaxUsages,
                    IsActive = payload.IsActive,
                    UserId = payload.UserId,
                    ExpiresAt = payload.ExpiresAt,
                    CreatedBy = userId,
                });

            await db.SaveChangesAsync();
        });

        return TypedResults.Ok(new CreatePromocodeResponse
        {
            Codes = codes.ToArray(),
        });
    }
}