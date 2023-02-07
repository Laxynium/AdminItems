using System.Net.Http.Json;
using AdminItems.Api.AdminItems.Features.GetAdminItems;
using AdminItems.Benchmarks.Shared;
using BenchmarkDotNet.Attributes;

namespace AdminItems.Benchmarks;

public class GetAdminItemsFromBeginning
{
    private PostgresDatabase _postgresDatabase;
    private AdminItemsApi _api;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        _postgresDatabase = new PostgresDatabase();
        await _postgresDatabase.InitializeAsync();

        await SeedBenchmarkData.SeedData(5000000, _postgresDatabase);
        
        _api = new AdminItemsApi(_postgresDatabase);
    }

    [Benchmark]
    public async Task PageOf200AdminItems()
    {
        var result = await _api.GetAdminItems((null, null, 200), ("orderBy", "code desc"));
        var response = await result.Content.ReadFromJsonAsync<Response>();
    }
    
    [Benchmark]
    public async Task First5Pages()
    {
        string[]? after = null;
        foreach (var i in Enumerable.Range(0, 5))
        {
            var result = await _api.GetAdminItems((after, null, 200), ("orderBy", "code desc"));
            var response = await result.Content.ReadFromJsonAsync<Response>();
            after = response.After;
        }
    }
    
    [Benchmark]
    public async Task First200Pages()
    {
        string[]? after = null;
        foreach (var i in Enumerable.Range(0, 200))
        {
            var result = await _api.GetAdminItems((after, null, 200), ("orderBy", "code desc"));
            var response = await result.Content.ReadFromJsonAsync<Response>();
            after = response.After;
        }
    }
    
    // [Benchmark]
    // public async Task ReadTillEnd()
    // {
    //     string? after = null;
    //     do
    //     {
    //         var result = await _api.GetAdminItems((after, null, 200), ("orderBy", "code desc"));
    //         var response = await result.Content.ReadFromJsonAsync<Response>();
    //         after = response.After;
    //     } while (after != null);
    // }
}