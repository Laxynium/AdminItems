using AdminItems.Api.AdminItems;
using AdminItems.Tests.Fakes;
using FluentAssertions;
using static AdminItems.Tests.Shared.Fixtures;

namespace AdminItems.Tests;

public class UpdateAdminItemEndpointTests
{
    [Fact]
    public async Task put_a_valid_admin_item()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(1);
        await apiFactory.ThereIsAnAdminItem(new
        {
            code = "ABCA123",
            name = "Some item name",
            colorId = DefaultColorId,
            comments = "Some comments"
        });

        var response = await apiFactory.PutAdminItem(1, new
        {
            code = "HGF123",
            name = "Another name",
            colorId = DefaultColorId,
            comments = "Some other comments"
        });

        response.Should().Be200Ok();
        adminItemsStore.Should().Contain(AdminItemId.Create(1), new AdminItem(
            "HGF123",
            "Another name",
            "Some other comments",
            DefaultColor));
    }

    [Fact]
    public async Task put_a_valid_admin_item_when_there_are_many_admin_items()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(1, 2, 3);
        await apiFactory.ThereIsAnAdminItem(new
        {
            code = "ABCA123",
            name = "Some item name",
            colorId = DefaultColorId,
            comments = "Some comments"
        });
        await apiFactory.ThereIsAnAdminItem(new
        {
            code = "GFDS123",
            name = "Second admin item",
            colorId = DefaultColorId,
            comments = "Another comment"
        });
        await apiFactory.ThereIsAnAdminItem(new
        {
            code = "HGFF312",
            name = "Thrid one",
            colorId = DefaultColorId,
            comments = "Yet another comment"
        });
        
        var response = await apiFactory.PutAdminItem(2, new
        {
            code = "UUA123",
            name = "Updated name",
            colorId = DefaultColorId,
            comments = "Some update comment"
        });

        response.Should().Be200Ok();
        adminItemsStore.Should().Contain(AdminItemId.Create(2), new AdminItem(
            "UUA123",
            "Updated name",
            "Some update comment",
            DefaultColor));
        adminItemsStore.Should().NotContain(AdminItemId.Create(2), new AdminItem(
            "GFDS123",
            "Second admin item",
            "Another comment",
            DefaultColor));
    }
}