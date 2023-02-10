using System.Net.Http.Json;
using System.Text.Json.Nodes;
using AdminItems.Api.Colors;
using AdminItems.IntegrationTests.Shared;
using FluentAssertions;

namespace AdminItems.IntegrationTests;

public class AdminItemTests : IntegrationTest
{
    public AdminItemTests(PostgresDatabase postgresDatabase) : base(postgresDatabase)
    {
    }

    [Fact]
    public async Task round_trip()
    {
        //given
        await ThereIsColor(new Color(3, "blue"));
        await ThereIsColor(new Color(5, "purple"));
        
        //when
        var response = await Api.PostAdminItem(new
        {
            code = "CODE_1",
            name = "admin_item_1",
            colorId = 3
        });
        response.Should().Be201Created();
        var id = await GetId(response);

        //then
        response = await Api.GetAdminItems();
        response.Should().Be200Ok().And
            .BeAs(new
            {
                items = new []
                {
                    new
                    {
                        id = id,
                        code = "CODE_1",
                        name = "admin_item_1",
                        color = "blue",
                    }
                }
            });
        
        //when
        response = await Api.PutAdminItem(id,new
        {
            code = "CODE_5",
            name = "admin_item_4",
            comments = "updated",
            colorId = 5
        });
        response.Should().Be200Ok();
        
        //then
        response = await Api.GetAdminItems();
        response.Should().Be200Ok().And
            .BeAs(new
            {
                items = new []
                {
                    new
                    {
                        id = id,
                        code = "CODE_5",
                        name = "admin_item_4",
                        color = "purple",
                    }
                }
            });
    }
    
    [Theory]
    [InlineData("CODE_1", "admin_item_2", 3)]
    [InlineData("CODE_3", "admin_item_1", 3)]
    public async Task conflict_when_duplicates_detected_on_add(string code, string name, int colorId)
    {
        await ThereIsColor(new Color(3, "blue"));
        await ThereIsColor(new Color(5, "purple"));
        await ThereIsAnAdminItem(new
        {
            code = "CODE_1",
            name = "admin_item_1",
            colorId = 3
        });

        var response = await Api.PostAdminItem(new
        {
            code = code,
            name = name,
            colorId = colorId
        });
        
        response.Should().Be409Conflict();
    }
    
    private async Task<long> ThereIsAnAdminItem(object request)
    {
        var response = await Api.PostAdminItem(request);
        response.Should().Be201Created();
        return await GetId(response);
    }

    private static async Task<long> GetId(HttpResponseMessage response)
    {
        var node = await response.Content.ReadFromJsonAsync<JsonNode>();
        return node!["id"]!.GetValue<long>();
    }
}