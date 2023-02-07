using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace AdminItems.IntegrationTests.Shared;

public class AdminItemsApi : WebApplicationFactory<Api.Program>
{
    private readonly PostgresDatabase _postgresDatabase;
    private HttpClient? _client;

    public AdminItemsApi(PostgresDatabase postgresDatabase)
    {
        _postgresDatabase = postgresDatabase;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("testing");
        builder.ConfigureAppConfiguration((ctx, cb) =>
        {
            cb.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "connectionStrings:postgres", _postgresDatabase.ConnectionString }
            });
        });
    }
    
    public async Task<HttpResponseMessage> GetAdminItems()
    {
        var client = GetClient();
        return await client.GetAsync("adminItems");
    }
    
    public async Task<HttpResponseMessage> GetColors()
    {
        var client = GetClient();
        return await client.GetAsync("colors");
    }
    
    public async Task<HttpResponseMessage> PostAdminItem(object request)
    {
        var client = GetClient();
        
        var serialized = JsonSerializer.Serialize(request);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        
        return await client.PostAsync("adminItems", content);
    }

    public async Task<HttpResponseMessage> PutAdminItem(long adminItemId, object request)
    {
        var client = GetClient();
        
        var serialized = JsonSerializer.Serialize(request);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        
        return await client.PutAsync($"adminItems/{adminItemId}", content);
    }
    
    private HttpClient GetClient() => _client ??= CreateClient();
}