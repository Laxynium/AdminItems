using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace AdminItems.IntegrationTests;

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
    
    private HttpClient GetClient() => _client ??= CreateClient();
}