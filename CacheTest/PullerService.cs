using CacheTest.Service;

namespace CacheTest;

internal class PullerService(ILogger<PullerService> logger, IDataService dataService) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var secondTimer = new PeriodicTimer(new TimeSpan(0, 0, 3));

		while (await secondTimer.WaitForNextTickAsync(stoppingToken))
		{
			await dataService.GetOtherData(Random.Shared.Next(5));
			logger.LogInformation("pulling done");
		}
	}
}