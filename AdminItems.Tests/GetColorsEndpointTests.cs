using FluentAssertions;

namespace AdminItems.Tests;

public class GetColorsEndpointTests
{
    [Fact]
    public async Task get_colors_with_success()
    {
        var apiFactory = new AdminItemsApi();

        var response = await apiFactory.GetColors();

        response.Should().Be200Ok();
    }
}