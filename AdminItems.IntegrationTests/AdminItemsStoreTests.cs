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
        var entity = await FindAdminItem(AdminItemId.Create(25));
        
        var updatedAdminItem = new AdminItem("AA123", "Item_Y_20", "Xyz", "azure");
        await Run<IAdminItemsStore>(store => store.Update(entity.WithValue(updatedAdminItem)));
        
        var adminItems = await GetAdminItems();
        adminItems.Should().HaveCount(1).And.ContainEquivalentOf(new AdminItemDto(
            25, 
            updatedAdminItem.Code, 
            updatedAdminItem.Name, 
            updatedAdminItem.Color, 
            updatedAdminItem.Comments));
    }

    [Fact]
    public async Task there_is_conflict_error_when_trying_to_update_stale_version_of_admin_item()
    {
        const uint maxUInt = 4_294_967_295;
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(maxUInt), new AdminItem("FAD123", "Item_X_15", "", "aqua")));
        
        var adminItem = await FindAdminItem(AdminItemId.Create(maxUInt));
        await Run<IAdminItemsStore>(store => store.Update(adminItem.WithValue(new AdminItem("AA123", "Item_X_15", "", "aqua"))));
        
        var action = () => Run<IAdminItemsStore>(store => store.Update(adminItem.WithValue(new AdminItem("BBB131", "Item_X_15", "", "aqua"))));

        await action.Should().ThrowAsync<OptimisticConcurrencyException>().WithMessage("*update*");
    }

    [Theory]
    [InlineData(15)]
    [InlineData(25)]
    public async Task find_admin_items_when_there_is_match(long id)
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(25), new AdminItem("FAD123", "Item_X_15", "", "aqua")));
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(15), new AdminItem("AAA123", "Item_Y_25", "X", "yellow")));
        
        var result = await Run<IAdminItemsStore, AdminItemEntity?>(store => store.Find(AdminItemId.Create(id)));

        result.Should().NotBeNull();
        result!.Id.Should().Be(AdminItemId.Create(id));
    }
    
    [Theory]
    [InlineData(145)]
    [InlineData(10)]
    public async Task find_admin_items_when_there_is_no_match(long id)
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(25), new AdminItem("FAD123", "Item_X_15", "", "aqua")));
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(15), new AdminItem("AAA123", "Item_Y_25", "X", "yellow")));
        
        var result = await Run<IAdminItemsStore, AdminItemEntity?>(store => store.Find(AdminItemId.Create(id)));

        result.Should().BeNull();
    }

    private async Task<List<AdminItemDto>> GetAdminItems()
    {
        await using var connection = new NpgsqlConnection(_postgresDatabase.ConnectionString);
        var result = await connection.QueryAsync<AdminItemDto>(
            "SELECT ai.id, ai.code, ai.name, ai.color, ai.comments  FROM admin_items ai");
        return result.ToList();
    }

    private async Task<AdminItemEntity> FindAdminItem(AdminItemId id)
    {
        var adminItem = await Run<IAdminItemsStore, AdminItemEntity?>(store => store.Find(id));
        adminItem.Should().NotBeNull();
        return adminItem!;
    }

    private record AdminItemDto(long Id, string Code, string Name, string Color, string Comments);
}