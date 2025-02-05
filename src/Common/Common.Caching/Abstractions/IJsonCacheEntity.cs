namespace Common.Caching.Abstractions;

public interface IJsonCacheEntity<T>
{
    Task<bool> ExistsAsync();
    Task<T?> GetAsync();
    Task SetAsync(T value);
    Task DeleteAsync();
}

public interface IKeyedJsonCacheEntity<TKey, TValue>
{
    Task<bool> ExistsAsync(TKey key);
    Task<TValue?> GetAsync(TKey key);
    Task SetAsync(TKey key, TValue value);
    Task DeleteAsync(TKey key);
}

public interface IListCacheEntity<T>
{
    Task<bool> ExistsAsync();
    Task<List<T>> GetAllAsync();
    Task SetAllAsync(List<T> items);
    Task LeftPushAsync(T item);
    Task RightPushAsync(T item);
    Task<int> GetCountAsync();
    Task ClearAsync();
    Task DeleteAsync();
}

public interface IKeyedListCacheEntity<TKey, TValue>
{
    Task<bool> ExistsAsync(TKey key);
    Task<List<TValue>> GetAllAsync(TKey key);
    Task SetAllAsync(TKey key, List<TValue> items);
    Task LeftPushAsync(TKey key, TValue item);
    Task RightPushAsync(TKey key, TValue item);
    Task<int> GetCountAsync(TKey key);
    Task ClearAsync(TKey key);
    Task DeleteAsync(TKey key);
}