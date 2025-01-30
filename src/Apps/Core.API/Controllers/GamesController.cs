using Core.Contracts.Compounds;
using Core.Db.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Contracts.Mappers;
using Core.Contracts.Public;
using Core.Contracts.Updates;
using Core.Services.Auth;
using Core.Services.Caching;
using Core.Services.Finance;
using Core.Services.PubSub;
using Core.Services.User;
using Microsoft.AspNetCore.Authorization;

namespace Core.API.Controllers;

public sealed record PlayDicePayload
{
    public required UserRole Role { get; init; }
}

public sealed record PlayDiceResponse
{
    public required UserPublic User { get; init; }
    public required UserDetails Profile { get; init; }
    public required Balance? Balance { get; init; }
    public required GameRecord? GameRecord { get; init; }
}

[ApiController]
public class GamesController(
    ILogger<GamesController> logger,
    CoreDbContext db,
    CoreCacheRegistry cache,
    CorePubSubRegistry pubSub,
    UserMapper userMapper,
    BalanceMapper balanceMapper,
    GameRecordMapper gameRecordMapper,
    BudgetService budgetService,
    ProfileService profileService) : ControllerBase
{
    [Authorize]
    [HttpPost("/games/play-dice")]
    [Consumes("application/json")]
    public async ValueTask<ActionResult<PlayDiceResponse>> PlayDice([FromBody] PlayDicePayload payload)
    {
        var userId = HttpContext.GetUserId();
        logger.LogInformation("UserId: {UserId}", userId);
        logger.LogInformation("Payload: {Payload}", payload);

        if (payload.Role == UserRole.Admin)
        {
            logger.LogInformation("ADMIN ROLE!");
        }

        var user = await db.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

        if (user == null)
        {
            logger.LogWarning("User {UserId} is not found", userId);
            return Unauthorized();
        }

        var balance = await cache.UserBalance.GetAsync(userId);
        var profile = await profileService.GetUserDetailsAsync(userId);

        var budget = await budgetService.GetBudgetAsync();

        logger.LogInformation("User {UserId} is playing dice!", userId);
        logger.LogInformation("User balance is {Balance}", balance);
        logger.LogInformation("User profile is {Profile}", profile);
        logger.LogInformation("Budget is {Budget}", budget);

        if (balance != null)
        {
            await pubSub.BalanceUpdated.PublishAsync(new UserUpdate<BalancePublic>
            {
                UserId = userId,
                Mode = UpdateMode.Optimized,
                Time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds(),
                Data = balanceMapper.ToPublic(balance)
            });
        }

        var gameRecord = await db.GameRecords.FirstOrDefaultAsync(r => r.UserId == userId);

        logger.LogInformation("Game record is {GameRecord}", gameRecord);
        logger.LogInformation("Game record snapshot is {Snapshot}", gameRecord?.Snapshot);

        var deposit = await db.Deposits.FirstOrDefaultAsync(d => d.UserId == userId);
        
        logger.LogInformation("Deposit is {Deposit}", deposit);
        logger.LogInformation("Deposit payload is {Payload}", deposit?.Payload);
        
        // var depositEntity = new DepositEntity()
        // {
        //     UserId = userId,
        //     Currency = Currency.RUB,
        //     Method = DepositMethod.CreditCard,
        //     Provider = PaymentProvider.Bovapay,
        //     ProviderTransactionId = "123456789",
        //     ProviderAmount = "100",
        //     Type = DepositType.Redirect,
        //     Payload = new DepositPayloadWhiteLabel
        //     {
        //         Instructions = new DepositInstructionsSbp
        //         {
        //             RecipientName = "Вася Пупкин",
        //             PhoneNumber = "+79991234567",
        //             ProviderAmount = "100",
        //         }
        //     }
        // };
        //
        // await db.Deposits.AddAsync(depositEntity);
        // await db.SaveChangesAsync();

        return Ok(new PlayDiceResponse
        {
            User = userMapper.ToPublic(user),
            Profile = profile,
            Balance = balance,
            GameRecord = gameRecord != null ? gameRecordMapper.ToData(gameRecord) : null
        });
    }
}