using Common.Db.Extensions;
using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services.Finance;

public class BalanceService(
    CoreDbContext db,
    CoreCacheRegistry cache,
    BalanceMapper balanceMapper,
    ILogger<BalanceService>? logger = null)
{
    public async Task<Balance> GetBalanceAsync(Guid userId, CancellationToken ct = default)
    {
        var cached = await cache.UserBalance.TryGetAsync(userId);
        if (cached is not null) return cached;

        var balanceEntity = await db.Balances
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.UserId == userId, ct);

        if (balanceEntity is null)
            throw new Exception("Balance not found");

        var balance = balanceMapper.ToData(balanceEntity);

        await cache.UserBalance.TrySetAsync(userId, balance);

        return balance;
    }

    public async Task<BalanceEntity> GetBalanceForUpdateAsync(Guid userId, CancellationToken ct = default)
    {
        // Log that we're looking up the balance for this user
        logger?.LogInformation("GetBalanceForUpdateAsync for user {UserId}", userId);
        
        // First check if the balance exists at all without ForUpdate
        var balanceCheck = await db.Balances
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.UserId == userId, ct);
            
        if (balanceCheck != null)
        {
            logger?.LogInformation("Balance exists with Available={Available}, fetching for update", balanceCheck.Available);
        }
        
        var balanceEntity = await db.Balances
            .ForUpdate()
            .FirstOrDefaultAsync(b => b.UserId == userId, ct);

        if (balanceEntity is null)
        {
            logger?.LogWarning("Balance not found for userId {UserId}", userId);
            throw new Exception("Balance not found");
        }

        logger?.LogInformation("GetBalanceForUpdateAsync returned: {UserId}, Available={Available}", 
            balanceEntity.UserId, balanceEntity.Available);

        return balanceEntity;
    }

    public async Task<TransactionEntity> CreateTransactionAsync(
        TransactionEntity transaction,
        CancellationToken ct = default)
    {
        var entry = await db.Transactions.AddAsync(transaction, ct);
        await db.SaveChangesAsync(ct);

        return entry.Entity;
    }

    public async Task<BalanceEntity> UpdateBalanceAsync(
        BalanceEntity balance,
        TransactionEntity transaction,
        GameRecordEntity? gameRecord = null,
        long wageringChange = 0)
    {
        if (!transaction.UserId.HasValue)
            throw new ArgumentException("UserId is required");
            
        logger?.LogInformation("UpdateBalanceAsync - Before: Available={Available}, Transaction Amount={Amount}", 
            balance.Available, transaction.Amount);

        balance.Available += transaction.Amount;
        balance.WageringRequired = Math.Max(0, balance.WageringRequired + wageringChange);
        
        logger?.LogInformation("UpdateBalanceAsync - After: Available={Available}", balance.Available);

        if (transaction.Type == TransactionType.Win)
        {
            if (gameRecord is null)
                throw new ArgumentException("GameRecord is required for game transactions");

            var won = transaction.Amount;
            balance.TotalBet += gameRecord.Bet;
            balance.TotalWon += won;
            balance.TotalBetCount++;

            if (won > balance.MaxWin)
            {
                balance.MaxWin = won;
                balance.MaxWinGameId = gameRecord.Id;
            }

            if (gameRecord.Multiplier > balance.MaxMultiplier)
            {
                balance.MaxMultiplier = gameRecord.Multiplier;
                balance.MaxMultiplierGameId = gameRecord.Id;
            }
        }

        if (transaction.Type == TransactionType.Loss)
        {
            if (gameRecord is null)
                throw new ArgumentException("GameRecord is required for game transactions");

            var lost = -transaction.Amount;
            balance.TotalBet += gameRecord.Bet;
            balance.TotalLost += lost;
            balance.TotalBetCount++;
        }

        db.Balances.Update(balance);
        await db.SaveChangesAsync();

        return balance;
    }
}