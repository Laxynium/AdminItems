using AdminItems.Api.AdminItems;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace AdminItems.IntegrationTests;

[Collection("Postgres Database")]
public class AdminItemsStoreTests
{
    private readonly PostgresDatabase _postgresDatabase;
    private AdminItemsApi _api;

    public AdminItemsStoreTests(PostgresDatabase postgresDatabase)
    {
        _postgresDatabase = postgresDatabase;
        _api = new AdminItemsApi(_postgresDatabase);
    }

    [Fact]
    public async Task admin_item_is_saved()
    {
        var adminItem = new AdminItem("ADDB123", "Item_1", "some comment", "red");
        
        await Run<IAdminItemsStore>(store => store.Add(15, adminItem));

        var adminItems = await GetAdminItems();
        adminItems.Should().HaveCount(1).And.ContainEquivalentOf(new AdminItemDto(
            15, 
            adminItem.Code, 
            adminItem.Name, 
            adminItem.Color, 
            adminItem.Comments));
    }

    private async Task<List<AdminItemDto>> GetAdminItems()
    {
        await using var connection = new NpgsqlConnection(_postgresDatabase.ConnectionString);
        var result = await connection.QueryAsync<AdminItemDto>(
            "SELECT ai.id, ai.code, ai.name, ai.color, ai.comments  FROM admin_items ai");
        return result.ToList();
    }

    private record AdminItemDto(long Id, string Code, string Name, string Color, string Comments);

    private async Task Run<TService>(Func<TService, Task> action) where TService : notnull
    {
        await using var scope = _api.Services.CreateAsyncScope();
        var store = scope.ServiceProvider.GetRequiredService<TService>();
        await action(store);
    }
}