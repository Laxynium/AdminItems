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
                        comments = ""
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
                        comments = "updated"
                    }
                }
            });
    }

    private static async Task<long> GetId(HttpResponseMessage response)
    {
        var node = await response.Content.ReadFromJsonAsync<JsonNode>();
        return node!["id"]!.GetValue<long>();
    }
}