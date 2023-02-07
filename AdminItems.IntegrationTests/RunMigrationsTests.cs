using AdminItems.IntegrationTests.Shared;
using Dapper;
using FluentAssertions;
using Npgsql;

namespace AdminItems.IntegrationTests;

public class RunMigrationsTests : IntegrationTest
{
    private readonly PostgresDatabase _postgresDatabase;

    public RunMigrationsTests(PostgresDatabase postgresDatabase):base(postgresDatabase)
    {
        _postgresDatabase = postgresDatabase;
    }
    
    [Fact]
    public async Task migrations_are_applied()
    {
        var api = new AdminItemsApi(_postgresDatabase);
        var client = api.CreateClient();

        using (var connection = new NpgsqlConnection(_postgresDatabase.ConnectionString))
        {
            var result = await connection.QueryAsync<AdminItemDto>(
                "SELECT ai.id, ai.code, ai.name, ai.color  FROM admin_items ai");
            result.Should().HaveCount(0);
        }
        
        using (var connection = new NpgsqlConnection(_postgresDatabase.ConnectionString))
        {
            var result = await connection.QueryAsync<ColorDto>(
                "SELECT c.id, c.name FROM colors c");
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

    private class ColorDto
    {
        public long Id { get; }
        public string Name { get; }

        public ColorDto(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}