using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AdminItems.Tests.InMemoryTests;

public class HomeEndpointTests
{
    [Fact]
    public async Task home_returns_ok()
    {
        var factory = new WebApplicationFactory<Api.Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("");

        response.Should().BeSuccessful();
    }
}