using AdminItems.Api.Colors;
using AdminItems.IntegrationTests.Shared;
using Dapper;
using FluentAssertions;
using Npgsql;

namespace AdminItems.IntegrationTests;

public class GetColorsTests : IntegrationTest
{
    public GetColorsTests(PostgresDatabase postgresDatabase) : base(postgresDatabase)
    {
    }

    [Fact]
    public async Task colors_defined_in_database_are_returned()
    {
        await ThereIsColor(new Color(5, "celadon"));
        await ThereIsColor(new Color(10, "ecru"));
        await ThereIsColor(new Color(13, "gold"));
        await ThereIsColor(new Color(17, "cerise"));
        
        var response = await Api.GetColors();

        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new { id = 5, name = "celadon" },
                    new { id = 10, name = "ecru" },
                    new { id = 13, name = "gold" },
                    new { id = 17, name = "cerise" },
                }
            });
    }

    private async Task ThereIsColor(Color color) =>
        await Run<NpgsqlConnection>(connection => connection.ExecuteAsync(
            "INSERT INTO colors(id, name) VALUES (@Id, @Name)",
            new { Id = color.Id, Name = color.Name }));
}