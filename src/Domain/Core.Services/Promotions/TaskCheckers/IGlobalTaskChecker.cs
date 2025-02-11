using Core.Contracts.Data;

namespace Core.Services.Promotions.TaskCheckers;

public interface IGlobalTaskChecker
{
    Task<GlobalTaskCheckResult> CheckAsync(Guid userId, GlobalTask task);
}