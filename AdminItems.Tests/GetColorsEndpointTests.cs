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

    [Fact]
    public async Task hardcoded_colors_are_returned()
    {
        var apiFactory = new AdminItemsApi();
        
        var response = await apiFactory.GetColors();

        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new []
                {
                    new {id = 1, name = "alizarin"},
                    new {id = 2, name = "amethyst"},
                    new {id = 3, name = "beige"},
                    new {id = 4, name = "cherry"},
                } 
            });
    }
}