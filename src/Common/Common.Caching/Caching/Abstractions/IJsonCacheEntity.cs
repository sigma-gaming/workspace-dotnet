using System.Text.Json;

namespace Common.Caching.Abstractions;

public interface IJsonCacheEntity<T>
{
    Task<T?> GetAsync();
    Task SetAsync(T value);
}

public interface IKeyedJsonCacheEntity<TKey, TValue>
{
    Task<TValue?> GetAsync(TKey key);
    Task SetAsync(TKey key, TValue value);
}

public interface IListCacheEntity<T>
{
    Task<List<T>> GetAllAsync();
    Task AddAsync(T item);
    Task<int> GetCountAsync();
    Task ClearAsync();
} 