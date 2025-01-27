using System.Text.Json;

namespace Common.Caching.Abstractions;

public interface IJsonCacheEntity<T>
{
    Task<T?> GetAsync();
    Task SetAsync(T value);
    Task DeleteAsync();
}

public interface IKeyedJsonCacheEntity<TKey, TValue>
{
    Task<TValue?> GetAsync(TKey key);
    Task SetAsync(TKey key, TValue value);
    Task DeleteAsync(TKey key);
}

public interface IListCacheEntity<T>
{
    Task<List<T>> GetAllAsync();
    Task AddAsync(T item);
    Task<int> GetCountAsync();
    Task ClearAsync();
    Task DeleteAsync();
} 

public interface IKeyedListCacheEntity<TKey, TValue>
{
    Task<List<TValue>> GetAllAsync(TKey key);
    Task AddAsync(TKey key, TValue item);
    Task<int> GetCountAsync(TKey key);
    Task ClearAsync(TKey key);
    Task DeleteAsync(TKey key);
}