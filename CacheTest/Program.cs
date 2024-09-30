using System.Runtime.CompilerServices;
using CacheTest;
using CacheTest.Caching;
using CacheTest.Service;
using Microsoft.AspNetCore.Mvc;

[assembly:InternalsVisibleTo("Tests")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCaching();
builder.Services.AddDataService();
builder.Services.AddHostedService<PullerService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/data", ([FromServices] IDataService dataService) => dataService.GetData())
	.WithName("get data")
	.WithOpenApi();

app.MapGet("/data/{id}", (int id, [FromServices] IDataService dataService) => dataService.GetOtherData(id))
	.WithName("get more data")
	.WithOpenApi();

app.MapPost("/invalidate", ([FromServices] DataServiceCacheDecorator cache) => cache.InvalidateData())
	.WithName("invalidate cache")
	.WithOpenApi();

app.MapPost("/invalidate/{id}", (int id, [FromServices] DataServiceCacheDecorator cache) => cache.InvalidateOtherData(id))
	.WithName("invalidate other cache")
	.WithOpenApi();

app.Run();