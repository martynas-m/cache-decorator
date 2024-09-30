namespace CacheTest.Service;

internal interface IDataService
{
	Task<Data> GetData();
	Task<Data[]> GetOtherData(int key);
}