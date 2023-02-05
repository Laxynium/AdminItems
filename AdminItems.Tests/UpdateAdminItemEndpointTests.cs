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
        adminItemsStore.Should().Contain(new AdminItem(
            "HGF123",
            "Another name",
            "Some other comments",
            DefaultColor));
    }
}