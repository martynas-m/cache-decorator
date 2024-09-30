namespace CacheTest.Service;

public static class DataServiceDIExtenssions
{
	public static IServiceCollection AddDataService(this IServiceCollection services)
	{
		services.AddTransient<IDataService, DataService>();
		services.AddTransient<DataServiceCacheDecorator>();
		services.Decorate<IDataService, DataServiceCacheDecorator>();
		return services;
	}
}