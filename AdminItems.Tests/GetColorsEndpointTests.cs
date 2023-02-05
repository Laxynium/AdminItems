using AdminItems.Api.Colors;
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
    public async Task colors_defined_in_colors_store_are_returned()
    {
        var colorsStore = new InMemoryColorsStore();
        colorsStore.AddColors(new []
        {
            new Color(5, "celadon"),
            new Color(10, "ecru"),
            new Color(13, "gold"),
            new Color(17, "cerise"),
        });
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(colorsStore);

        var response = await apiFactory.GetColors();
        
        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new []
                {
                    new {id = 5, name = "celadon"},
                    new {id = 10, name = "ecru"},
                    new {id = 13, name = "gold"},
                    new {id = 17, name = "cerise"},
                } 
            });
    }
}