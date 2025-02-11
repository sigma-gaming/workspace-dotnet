using System.ComponentModel.DataAnnotations;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Contracts.Public;
using Core.Contracts.Updates;
using Core.Services.Auth;
using Core.Services.Promotions;
using Core.Services.PubSub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers;

public sealed record ApplyPromocodePayload
{
    [Required] public required string Code { get; init; }
}

public sealed record ApplyPromocodeResponse
{
    public required PromocodeBonusType BonusType { get; init; }
    public required long Payout { get; init; }
    public required UserUpdate<BalancePublic> Balance { get; init; }
}

[Authorize]
[ApiController]
public class PromocodesController(
    CorePubSubRegistry pubSub,
    BalanceMapper balanceMapper,
    PromocodeService promocodeService) : ControllerBase
{
    [ProducesResponseType<ApplyPromocodeResponse>(StatusCodes.Status200OK)]
    [HttpPost("/promocodes/apply")]
    public async Task<Results<Ok<ApplyPromocodeResponse>, ValidationProblem>> Apply(
        [FromBody] ApplyPromocodePayload payload)
    {
        var userId = HttpContext.GetUserId();

        var result = await promocodeService.ApplyPayoutAsync(userId, payload.Code);

        switch (result)
        {
            case PromocodeActivationPayoutResult payout:
                var balanceUpdate = new UserUpdate<BalancePublic>
                {
                    UserId = userId,
                    Mode = UpdateMode.Optimized,
                    Data = balanceMapper.ToPublic(payout.UpdatedBalance),
                };

                await pubSub.BalanceUpdated.PublishAsync(balanceUpdate);

                return TypedResults.Ok(new ApplyPromocodeResponse
                {
                    BonusType = PromocodeBonusType.Payout,
                    Payout = payout.Payout,
                    Balance = balanceUpdate,
                });

            case PromocodeActivationWrongUsageResult wrongUsage:
                var wrongUsageMessage = wrongUsage.BonusType switch
                {
                    PromocodeBonusType.DepositFixed => "Данный промокод нужно использовать при пополнении",
                    PromocodeBonusType.DepositMultiplier => "Данный промокод нужно использовать при пополнении",
                    PromocodeBonusType.Payout => "Данный промокод нужно использовать на странице бонусов",
                    _ => "Неверный тип промокода",
                };

                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(ApplyPromocodePayload.Code)] = [wrongUsageMessage],
                });

            default:
                var failureMessage = result.Outcome switch
                {
                    PromocodeActivationOutcome.NotFound => "Промокод не найден",
                    PromocodeActivationOutcome.Expired => "Срок действия промокода истёк",
                    PromocodeActivationOutcome.UsageExceeded => "Достигнут лимит использования промокода",
                    PromocodeActivationOutcome.AlreadyUsed => "Вы уже использовали этот промокод",
                    PromocodeActivationOutcome.Inactive => "Промокод не активен",
                    PromocodeActivationOutcome.Blocked => "Промокод заблокирован",
                    _ => "Не удалось применить промокод. Попробуйте ещё раз или напишите в поддержку",
                };

                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(ApplyPromocodePayload.Code)] = [failureMessage],
                });
        }
    }
}