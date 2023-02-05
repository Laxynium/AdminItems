using System.Text;
using System.Text.Json;
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

    private async Task<HttpResponseMessage> PostAdminItem(object request)
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var serialized = JsonSerializer.Serialize(request);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        
        return await client.PostAsync("adminItems", content);
    }
}