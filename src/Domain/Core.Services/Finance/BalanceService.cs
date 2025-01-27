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
    ILogger<BalanceService> logger,
    BalanceMapper balanceMapper,
    TransactionMapper transactionMapper)
{
    public async Task<Balance> GetBalanceAsync(Guid userId, CancellationToken ct = default)
    {
        if (cache.Ready())
        {
            var cached = await cache.UserBalance.GetAsync(userId);
            if (cached != null) return cached;
        }
        
        var balanceEntity = await db.Balances
            .FirstOrDefaultAsync(b => b.UserId == userId, ct)
            ?? throw new Exception("Balance not found");

        var balance = balanceMapper.ToData(balanceEntity);
        
        if (cache.Ready())
            await cache.UserBalance.SetAsync(userId, balance);

        return balance;
    }

    public async Task<Transaction> CreateTransactionAsync(
        TransactionEntity transaction,
        CancellationToken ct = default)
    {
        var entry = await db.Transactions.AddAsync(transaction, ct);
        await db.SaveChangesAsync(ct);

        return transactionMapper.ToData(entry.Entity);
    }

    public async Task<Balance> UpdateBalanceAsync(
        BalanceEntity balance,
        TransactionEntity transaction,
        GameRecordEntity? gameRecord = null,
        long wageringChange = 0,
        CancellationToken ct = default)
    {
        if (!transaction.UserId.HasValue)
            throw new ArgumentException("UserId is required");

        balance.Available += transaction.Amount;
        balance.WageringRequired = Math.Max(0, balance.WageringRequired + wageringChange);

        if (transaction.Type == TransactionType.Win)
        {
            if (gameRecord == null)
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
            if (gameRecord == null)
                throw new ArgumentException("GameRecord is required for game transactions");

            var lost = -transaction.Amount;
            balance.TotalBet += gameRecord.Bet;
            balance.TotalLost += lost;
            balance.TotalBetCount++;
        }

        db.Balances.Update(balance);
        await db.SaveChangesAsync(ct);

        return balanceMapper.ToData(balance);
    }
}