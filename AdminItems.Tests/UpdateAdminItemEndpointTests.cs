using AdminItems.Api.AdminItems;
using AdminItems.Tests.Fakes;
using FluentAssertions;
using static AdminItems.Tests.Shared.Fixtures;

namespace AdminItems.Tests;

public class UpdateAdminItemEndpointTests
{
    [Theory]
    [InlineData("GFJS1234", "First Admin Item", "This is a first admin item in system")]
    [InlineData("YTRA1235", "Another admin Item", "")]
    [InlineData("1234567890AB", "Another one", "Admin item with max 12 characters")]
    public async Task put_a_valid_admin_item(string? code, string? name, string? comments)
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
            code = code,
            name = name,
            colorId = DefaultColorId,
            comments = comments
        });

        response.Should().Be200Ok();
        adminItemsStore.Should().Contain(AdminItemId.Create(1), new AdminItem(
            code!,
            name!,
            comments!,
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