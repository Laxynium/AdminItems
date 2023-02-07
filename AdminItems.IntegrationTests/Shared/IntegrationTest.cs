using Microsoft.Extensions.DependencyInjection;

namespace AdminItems.IntegrationTests.Shared;

[Collection("Postgres Database")]
public class IntegrationTest : IAsyncLifetime
{
    private readonly PostgresDatabase _postgresDatabase;
    protected readonly  AdminItemsApi Api;

    protected IntegrationTest(PostgresDatabase postgresDatabase)
    {
        _postgresDatabase = postgresDatabase;
        Api = new AdminItemsApi(_postgresDatabase);
    }
    Task IAsyncLifetime.InitializeAsync() => _postgresDatabase.CleanUp();

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
    
    protected async Task Run<TService>(Func<TService, Task> action) where TService : notnull
    {
        await using var scope = Api.Services.CreateAsyncScope();
        var store = scope.ServiceProvider.GetRequiredService<TService>();
        await action(store);
    }
    protected async Task<TResult> Run<TService, TResult>(Func<TService, Task<TResult>> action) where TService : notnull
    {
        await using var scope = Api.Services.CreateAsyncScope();
        var store = scope.ServiceProvider.GetRequiredService<TService>();
        return await action(store);
    }
}