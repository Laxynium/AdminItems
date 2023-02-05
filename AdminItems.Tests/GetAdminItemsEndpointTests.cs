using AdminItems.Tests.Fakes;
using FluentAssertions;
using static AdminItems.Tests.Shared.Fixtures;

namespace AdminItems.Tests;

public class GetAdminItemsEndpointTests
{
    [Theory]
    [InlineData("ADBD123", "Some random admin item name", null)]
    public async Task response_contains_created_admin_item(string code, string name, string? comments)
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var api = AnAdminItemsApi(adminItemsStore);
        await api.PostAdminItem(new
        {
            code,
            name,
            DefaultColorId,
            comments
        });

        var response = await api.GetAdminItems();

        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new
                    {
                        code,
                        name,
                        color = DefaultColor,
                        comments = comments ?? string.Empty
                    }
                }
            });
    }
}