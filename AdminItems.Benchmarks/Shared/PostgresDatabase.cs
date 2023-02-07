using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Respawn;
using Respawn.Graph;

namespace AdminItems.Benchmarks.Shared;

public class PostgresDatabase
{
    private readonly PostgresContainerWrapper _postgresqlContainer = new(true);
    private Respawner? _respawner;

    public string ConnectionString => _postgresqlContainer.ConnectionString;
    public async Task InitializeAsync()
    {
        await _postgresqlContainer.StartAsync();

        var migrator = Migrator.Migrator.Create(ConnectionString, "Migrations", new NullLoggerFactory());
        migrator.Migrate();
        
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new []{"public"},
            TablesToIgnore = new []
            {
                new Table("schemaversions")
            }
        });
    }
    
    public async Task CleanUp()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await _respawner!.ResetAsync(connection);
    }
    
    public Task DisposeAsync() => _postgresqlContainer.DisposeAsync().AsTask();

    private class PostgresContainerWrapper : IAsyncDisposable
    {
        private TestcontainerDatabase? _postgresqlContainer;
        private readonly bool _useExistingContainer;

        public PostgresContainerWrapper(bool useExistingContainer)
        {
            _useExistingContainer = useExistingContainer;
        }

        public string ConnectionString => _postgresqlContainer?.ConnectionString
            ?? "Server=127.0.0.1;Port=5432;UserId=postgres;Password=postgres;Database=admin_items_db_perf_tests";
        public async Task StartAsync()
        {
            if (_useExistingContainer)
            {
                return;
            }

            /*https://github.com/testcontainers/testcontainers-dotnet/issues/750#issuecomment-1412257694*/
#pragma warning disable 618
            _postgresqlContainer = new ContainerBuilder<PostgreSqlTestcontainer>()
                .WithDatabase(new PostgreSqlTestcontainerConfiguration
                {
                    Database = "admin_items_db",
                    Username = "postgres",
                    Password = "postgres"
                })
                .Build();
#pragma warning restore 618
            await _postgresqlContainer.StartAsync();
            
        }
        
        public async ValueTask DisposeAsync()
        {
            if (_postgresqlContainer is not null)
                await _postgresqlContainer.DisposeAsync();
        }
    }
}