using AdminItems.Tests.Fakes;
using FluentAssertions;
using static AdminItems.Tests.Shared.Fixtures;

namespace AdminItems.Tests;

public class GetAdminItemsEndpointTests
{
    [Fact]
    public async Task response_contains_created_admin_item()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var api = AnAdminItemsApi(adminItemsStore);
        await api.PostAdminItem(new
        {
            code = "ADBD123",
            name = "Some random admin item name",
            colorId = DefaultColorId,
            comments = (string?)null
        });

        var response = await api.GetAdminItems();

        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new
                    {
                        code = "ADBD123",
                        name = "Some random admin item name",
                        color = DefaultColor,
                        comments = string.Empty
                    }
                }
            });
    }
}