namespace AdminItems.IntegrationTests;

[Collection("Postgres Database")]
public class IntegrationTest : IAsyncLifetime
{
    private readonly PostgresDatabase _postgresDatabase;

    public IntegrationTest(PostgresDatabase postgresDatabase)
    {
        _postgresDatabase = postgresDatabase;
    }
    Task IAsyncLifetime.InitializeAsync() => _postgresDatabase.CleanUp();

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}