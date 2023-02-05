using System.Text;
using System.Text.Json;
using AdminItems.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AdminItems.Tests;

public class CreateAdminItemEndpointTests
{
    [Fact]
    public async Task post_a_valid_admin_item()
    {
        var request = new
        {
            code = "GFJS1234",
            name = "First Admin Item",
            comments = "This is a first admin item in system"
        };

        var response = await PostAdminItem(request);

        response.Should().BeSuccessful();
    }

    [Theory]
    [InlineData("GFJS1234", "First Admin Item", "This is a first admin item in system")]
    public async Task valid_admin_item_is_added_to_store(string code, string name, string comments)
    {
        var fakeStore = new FakeAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(fakeStore);

        var request = new { code, name, comments };
        var response = await apiFactory.PostAdminItem(request);;

        response.Should().BeSuccessful();
        fakeStore.Should().Contain(new AdminItem(
            request.code,
            request.name,
            request.comments));
    }

    private async Task<HttpResponseMessage> PostAdminItem(object request)
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var serialized = JsonSerializer.Serialize(request);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        
        return await client.PostAsync("adminItems", content);
    }
}