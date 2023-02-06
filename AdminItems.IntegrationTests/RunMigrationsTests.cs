using Dapper;
using FluentAssertions;
using Npgsql;

namespace AdminItems.IntegrationTests;

[Collection("Postgres Database")]
public class RunMigrationsTests
{
    private readonly PostgresDatabase _postgresDatabase;

    public RunMigrationsTests(PostgresDatabase postgresDatabase)
    {
        _postgresDatabase = postgresDatabase;
    }
    
    [Fact]
    public async Task postgres_database_is_running_migrations_without_errors()
    {
        var api = new AdminItemsApi(_postgresDatabase);
        var client = api.CreateClient();

        using (var connection = new NpgsqlConnection(_postgresDatabase.ConnectionString))
        {
            var result = await connection.QueryAsync<AdminItemDto>(
                "SELECT ai.id, ai.code, ai.name, ai.color  FROM admin_items ai");
            result.Should().HaveCount(0);
        }
    }

    private class AdminItemDto
    {
        public long Id { get; }
        public string Code { get; }
        public string Name { get; }
        public string Color { get; }

        public AdminItemDto(long id, string code, string name, string color)
        {
            Id = id;
            Code = code;
            Name = name;
            Color = color;
        }
    }
}