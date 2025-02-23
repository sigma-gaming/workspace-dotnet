namespace Common.Caching.Abstractions;

public interface IJsonCacheEntity<T> : ICacheOperations where T : class
{
    Task<T?> GetAsync();
    Task<T?> TryGetAsync();
    Task SetAsync(T value);
    Task TrySetAsync(T value);
}

public interface IKeyedJsonCacheEntity<in TKey, T> : IKeyedCacheOperations<TKey> where T : class
{
    Task<T?> GetAsync(TKey key);
    Task<T?> TryGetAsync(TKey key);
    Task SetAsync(TKey key, T value);
    Task TrySetAsync(TKey key, T value);
}

public interface IListCacheEntity<T> : ICacheOperations where T : class
{
    Task<List<T>> GetAllAsync();
    Task<(List<T> list, bool succeed)> TryGetAllAsync();
    Task SetAllAsync(List<T> items);
    Task TrySetAllAsync(List<T> items);
    Task LeftPushAsync(T item);
    Task TryLeftPushAsync(T item);
    Task RightPushAsync(T item);
    Task TryRightPushAsync(T item);
    Task<int> GetCountAsync();
    Task<(int count, bool succeed)> TryGetCountAsync();
    Task ClearAsync();
    Task TryClearAsync();
}

public interface IKeyedListCacheEntity<in TKey, T> : IKeyedCacheOperations<TKey> where T : class
{
    Task<List<T>> GetAllAsync(TKey key);
    Task<(List<T> list, bool succeed)> TryGetAllAsync(TKey key);
    Task SetAllAsync(TKey key, List<T> items);
    Task TrySetAllAsync(TKey key, List<T> items);
    Task LeftPushAsync(TKey key, T item);
    Task TryLeftPushAsync(TKey key, T item);
    Task RightPushAsync(TKey key, T item);
    Task TryRightPushAsync(TKey key, T item);
    Task<int> GetCountAsync(TKey key);
    Task<(int count, bool succeed)> TryGetCountAsync(TKey key);
    Task ClearAsync(TKey key);
    Task TryClearAsync(TKey key);
}