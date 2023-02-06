using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Respawn;
using Respawn.Graph;

namespace AdminItems.IntegrationTests;

public class PostgresDatabase : IAsyncLifetime
{
    /*https://github.com/testcontainers/testcontainers-dotnet/issues/750#issuecomment-1412257694*/
#pragma warning disable 618
    private readonly TestcontainerDatabase _postgresqlContainer = new ContainerBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "admin_items_db",
            Username = "postgres",
            Password = "postgres"
        })
        .Build();
#pragma warning restore 618

    private Respawner? _respawner;
    public string ConnectionString => _postgresqlContainer.ConnectionString;
    
    public async Task InitializeAsync()
    {
        await _postgresqlContainer.StartAsync();

        var migrator = Migrator.Migrator.Create(ConnectionString, "Migrations", new NullLoggerFactory());
        migrator.Migrate();
        
        await using var connection = new NpgsqlConnection(_postgresqlContainer.ConnectionString);
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
        await using var connection = new NpgsqlConnection(_postgresqlContainer.ConnectionString);
        await connection.OpenAsync();
        await _respawner!.ResetAsync(connection);
    }
    
    public Task DisposeAsync()
    {
        return _postgresqlContainer.DisposeAsync().AsTask();
    }
}

[CollectionDefinition("Postgres Database", DisableParallelization = true)]
public class PostgresDatabaseCollection : ICollectionFixture<PostgresDatabase>
{
}