using CacheTest;
using CacheTest.Caching;
using CacheTest.Service;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Tests;

public class CacheLoadingTests
{
	private readonly record struct DataKey(int Id) : CacheInvalidator.ICacheKey
	{
		public static DataKey FromId(int id) => new(id);
		public string Key => $"{nameof(DataServiceCacheDecorator)}_{Id}";
	}
	
	[Fact]
	public async Task Should_load_once()
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddCaching();
		var sp = serviceCollection.BuildServiceProvider();
		var cache = sp.GetRequiredService<MemoryCacheLoadingWrapper>();

		var l1 = cache.GetOrLoad(DataKey.FromId(1), () => Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ => 1));
		var l2 = cache.GetOrLoad(DataKey.FromId(2), () => Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ => "2"));
		var l3 = cache.GetOrLoad(DataKey.FromId(3), () => Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ => 1));
		var l4 = cache.GetOrLoad(DataKey.FromId(4), () => Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ => "2"));
		var l5 = cache.GetOrLoad(DataKey.FromId(4), () => Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ => "2"));

		Func<Task> allLoad = () => Task.WhenAll(l1, l2, l3, l4, l5);

		await allLoad.Should().CompleteWithinAsync(4.Seconds());
	}
}