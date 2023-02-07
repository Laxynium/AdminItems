using System.Net.Http.Json;
using AdminItems.Api.AdminItems.Features.GetAdminItems;
using AdminItems.Benchmarks.Shared;
using BenchmarkDotNet.Attributes;

namespace AdminItems.Benchmarks;

public class GetAdminItemsFromTheMiddleOnColor
{
    private PostgresDatabase _postgresDatabase;
    private AdminItemsApi _api;
    private string[] _before;
    private string[] _after;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        _postgresDatabase = new PostgresDatabase();
        await _postgresDatabase.InitializeAsync();

        await SeedBenchmarkData.SeedData(5000000, _postgresDatabase);
        
        _api = new AdminItemsApi(_postgresDatabase);

        (_before, _after) = await MoveToMiddle();
    }

    [Benchmark]
    public async Task PageOf200AdminItemsGoingBackwards()
    {
        var result = await _api.GetAdminItems((_before, null, 200), ("orderBy", "color desc"));
        var response = await result.Content.ReadFromJsonAsync<Response>();
    }
    
    [Benchmark]
    public async Task First5PagesGoingBackwards()
    {
        string[]? after = null;
        foreach (var i in Enumerable.Range(0, 5))
        {
            var result = await _api.GetAdminItems((_before, null, 200), ("orderBy", "color desc"));
            var response = await result.Content.ReadFromJsonAsync<Response>();
            after = response.After;
        }
    }
    
    private async Task<(string[]before, string[]after)> MoveToMiddle()
    {
        string[]? before = null;
        string[]? after = null;
        foreach (var i in Enumerable.Range(0, 4000))
        {
            var result = await _api.GetAdminItems((after, null, 200), ("orderBy", "color desc"));
            var response = await result.Content.ReadFromJsonAsync<Response>();
            after = response.After;
            before = response.Before;
        }

        return (before, after);
    }
}