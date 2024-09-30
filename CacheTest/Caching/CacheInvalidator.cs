using System.Collections.Concurrent;

namespace CacheTest.Caching;

public class CacheInvalidator
{
	private readonly ConcurrentDictionary<string, IList<CancellationTokenSource>> _invalidations = new();

	public void AddInvalidator(ICacheKey key, CancellationTokenSource invalidator) =>
		_invalidations.AddOrUpdate(key.Key, [invalidator], (_, tokens) =>
		{
			tokens.Add(invalidator);
			return tokens;
		});

	public void Invalidate(ICacheKey key)
	{
		if (!_invalidations.TryRemove(key.Key, out var expirationTokens)) return;

		foreach (var expirationToken in expirationTokens)
			expirationToken.Cancel();
	}

	public interface ICacheKey
	{
		string Key { get; }
	}
}