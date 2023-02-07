using AdminItems.Api.AdminItems;
using AdminItems.IntegrationTests.Shared;
using Dapper;
using FluentAssertions;
using Npgsql;

namespace AdminItems.IntegrationTests;

public class AdminItemsStoreTests : IntegrationTest
{
    private readonly PostgresDatabase _postgresDatabase;

    public AdminItemsStoreTests(PostgresDatabase postgresDatabase):base(postgresDatabase)
    {
        _postgresDatabase = postgresDatabase;
    }

    [Fact]
    public async Task admin_item_is_saved()
    {
        var adminItem = new AdminItem("ADDB123", "Item_1", "some comment", "red");
        
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(15), adminItem));

        var adminItems = await GetAdminItems();
        adminItems.Should().HaveCount(1).And.ContainEquivalentOf(new AdminItemDto(
            15, 
            adminItem.Code, 
            adminItem.Name, 
            adminItem.Color, 
            adminItem.Comments));
    }
    
    [Fact]
    public async Task admin_item_is_updated()
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(25), new AdminItem("FAD123", "Item_X_15", "", "aqua")));

        var adminItem = new AdminItem("AA123", "Item_Y_20", "Xyz", "azure");
        await Run<IAdminItemsStore>(store => store.Update(AdminItemId.Create(25), adminItem));
        
        var adminItems = await GetAdminItems();
        adminItems.Should().HaveCount(1).And.ContainEquivalentOf(new AdminItemDto(
            25, 
            adminItem.Code, 
            adminItem.Name, 
            adminItem.Color, 
            adminItem.Comments));
    }

    [Theory]
    [InlineData(15, true)]
    [InlineData(25, true)]
    [InlineData(123, false)]
    public async Task contains_admin_items(long id, bool expectedContains)
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(25), new AdminItem("FAD123", "Item_X_15", "", "aqua")));
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(15), new AdminItem("AAA123", "Item_Y_25", "X", "yellow")));
        
        (await Run<IAdminItemsStore, bool>(store => store.Contains(AdminItemId.Create(id)))).Should().Be(expectedContains);
    }

    private async Task<List<AdminItemDto>> GetAdminItems()
    {
        await using var connection = new NpgsqlConnection(_postgresDatabase.ConnectionString);
        var result = await connection.QueryAsync<AdminItemDto>(
            "SELECT ai.id, ai.code, ai.name, ai.color, ai.comments  FROM admin_items ai");
        return result.ToList();
    }
    
    private record AdminItemDto(long Id, string Code, string Name, string Color, string Comments);
}