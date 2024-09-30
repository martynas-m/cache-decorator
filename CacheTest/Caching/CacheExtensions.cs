namespace CacheTest.Caching;

internal static class CacheExtensions
{
	private static readonly CacheInvalidator CacheInvalidator = new ();
	public static IServiceCollection AddCaching(this IServiceCollection services)
	{
		services.AddMemoryCache();
		services.AddSingleton(_ => CacheInvalidator);
		services.AddTransient<MemoryCacheLoadingWrapper>();

		return services;
	}
}