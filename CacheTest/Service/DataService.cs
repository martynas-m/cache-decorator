namespace CacheTest.Service;

internal class DataService(ILogger<DataService> logger) : IDataService
{
	public async Task<Data> GetData()
	{
		logger.LogWarning("Loading data");
		await Task.Delay(TimeSpan.FromSeconds(30));
		return new Data("loaded data", DateTime.Now);
	}
	
	public async Task<Data[]> GetOtherData(int key)
	{
		logger.LogWarning("Loading other data by {Key}", key);
		await Task.Delay(TimeSpan.FromSeconds(30));
		return Enumerable
			.Range(1, Random.Shared.Next(4, 10))
			.Select(index => new Data(index.ToString(), DateTime.Now))
			.ToArray();
	}
}