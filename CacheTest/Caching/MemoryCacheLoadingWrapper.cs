using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CacheTest.Caching;

internal class MemoryCacheLoadingWrapper(IMemoryCache cache, CacheInvalidator cacheInvalidator)
{
	private static readonly ConcurrentDictionary<CacheInvalidator.ICacheKey, SemaphoreSlim> Semaphores = new();
	private readonly TimeSpan _defaultTimeout = TimeSpan.FromMinutes(5);

	public async Task<TResult> GetOrLoad<TResult>(CacheInvalidator.ICacheKey key, Func<Task<TResult>> loader)
	{
		if (cache.TryGetValue<TResult>(key, out var result)) return result!;

		var semaphore = Semaphores.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
		try
		{
			await semaphore.WaitAsync();

			if (cache.TryGetValue(key, out result)) return result!;

			result = await loader();

			cache.Set(key, result, GetCacheOptions(key, _defaultTimeout));
			return result;
		}
		finally
		{
			semaphore.Release();
		}
	}

	public void Set<TData>(CacheInvalidator.ICacheKey key, TData data) => cache.Set(key, data, GetCacheOptions(key, _defaultTimeout));

	public void Invalidate(CacheInvalidator.ICacheKey key) => cacheInvalidator.Invalidate(key);

	private MemoryCacheEntryOptions GetCacheOptions(CacheInvalidator.ICacheKey key, TimeSpan? expirationTimeout)
	{
		var cts = new CancellationTokenSource();
		var expiration = new CancellationChangeToken(cts.Token);
		cacheInvalidator.AddInvalidator(key, cts);
		var options = new MemoryCacheEntryOptions
		{
			ExpirationTokens = { expiration }
		};

		if(expirationTimeout.HasValue)
			options.AbsoluteExpirationRelativeToNow = expirationTimeout;

		return options;
	}
}