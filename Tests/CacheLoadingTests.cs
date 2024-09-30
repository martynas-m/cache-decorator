using CacheTest.Caching;
using CacheTest.Service;
using FluentAssertions;
using FluentAssertions.Extensions;

namespace Tests;

public class CacheLoadingTests(CacheTestSetup setup) : IClassFixture<CacheTestSetup>
{
	private readonly record struct DataKey(int Id) : CacheInvalidator.ICacheKey
	{
		public static DataKey FromId(int id) => new(id);
		public string Key => $"{nameof(DataServiceCacheDecorator)}_{Id}";
	}

	[Fact]
	public async Task Should_load_asynchronously()
	{
		var cache = setup.GetRequiredService<MemoryCacheLoadingWrapper>();

		var l1 = cache.GetOrLoad(DataKey.FromId(1), () => Task.Delay(3.Seconds()).ContinueWith(_ => 1));
		var l2 = cache.GetOrLoad(DataKey.FromId(2), () => Task.Delay(2.Seconds()).ContinueWith(_ => "2"));
		var l3 = cache.GetOrLoad(DataKey.FromId(3), () => Task.Delay(3.Seconds()).ContinueWith(_ => 1));
		var l4 = cache.GetOrLoad(DataKey.FromId(4), () => Task.Delay(3.Seconds()).ContinueWith(_ => "2"));
		var l5 = cache.GetOrLoad(DataKey.FromId(4), () => Task.Delay(3.Seconds()).ContinueWith(_ => "2"));

		Func<Task> allLoad = () => Task.WhenAll(l1, l2, l3, l4, l5);

		await allLoad.Should().CompleteWithinAsync(4.Seconds());
	}

	[Fact]
	public async Task Should_load_once()
	{
		var calls = 0;
		var cache = setup.GetRequiredService<MemoryCacheLoadingWrapper>();

		var l1 = cache.GetOrLoad(DataKey.FromId(1), () => Task.Delay(1.Seconds()).ContinueWith(_ => Interlocked.Increment(ref calls)));
		var l2 = cache.GetOrLoad(DataKey.FromId(1), () => Task.Delay(1.Seconds()).ContinueWith(_ => Interlocked.Increment(ref calls)));
		
		await Task.WhenAll(l1, l2);
		calls.Should().Be(1);
	}
}