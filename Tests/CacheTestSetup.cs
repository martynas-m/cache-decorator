using CacheTest.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class CacheTestSetup
{
	private readonly IServiceProvider _serviceProvider;

	public CacheTestSetup()
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddCaching();
		_serviceProvider = serviceCollection.BuildServiceProvider();
	}

	public TService GetRequiredService<TService>() where TService : notnull =>
		_serviceProvider.GetRequiredService<TService>();
}