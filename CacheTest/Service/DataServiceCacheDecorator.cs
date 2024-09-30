using CacheTest.Caching;

namespace CacheTest.Service;

internal class DataServiceCacheDecorator(IDataService decorated, MemoryCacheLoadingWrapper cache) : IDataService
{
	private readonly record struct DataKey : CacheInvalidator.ICacheKey
	{
		public static DataKey Instance { get; } = new();
		public string Key => $"{nameof(DataServiceCacheDecorator)}";
	}

	private readonly record struct IntegerKey(int Value) : CacheInvalidator.ICacheKey
	{
		public static IntegerKey From(int value) => new (value);
		public string Key => $"{nameof(DataServiceCacheDecorator)}_{Value}";
	}

	public Task<Data> GetData() => cache.GetOrLoad(DataKey.Instance, decorated.GetData);

	public Task<Data[]> GetOtherData(int id) => cache.GetOrLoad(IntegerKey.From(id), () => decorated.GetOtherData(id));

	public void InvalidateData() => cache.Invalidate(DataKey.Instance);

	public void InvalidateOtherData(int id) => cache.Invalidate(IntegerKey.From(id));
}