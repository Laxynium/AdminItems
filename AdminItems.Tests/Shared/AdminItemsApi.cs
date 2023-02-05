using System.Text;
using System.Text.Json;
using AdminItems.Api.AdminItems;
using AdminItems.Api.Colors;
using AdminItems.Tests.Fakes;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AdminItems.Tests.Shared;

public class AdminItemsApi : WebApplicationFactory<Api.Program>
{
    private HttpClient? _client;
    private IAdminItemsStore _adminItemsStore = new InMemoryAdminItemsStore();
    private IColorsStore _colorsStore = new InMemoryColorsStore();
    
    public void UseStore(IAdminItemsStore inMemoryAdminItemsStore)
    {
        _adminItemsStore = inMemoryAdminItemsStore;
    }

    public void UseStore(IColorsStore inMemoryColorsStore)
    {
        _colorsStore = inMemoryColorsStore;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAdminItemsStore>();
            services.TryAddSingleton(_adminItemsStore);

            services.RemoveAll<IColorsStore>();
            services.TryAddSingleton(_colorsStore);
        });
    }

    public async Task<HttpResponseMessage> ThereIsAnAdminItem(object request)
    {
        var response = await PostAdminItem(request);
        response.Should().Be200Ok();
        return response;
    }

    public async Task<HttpResponseMessage> PostAdminItem(object request)
    {
        var client = GetClient();
        
        var serialized = JsonSerializer.Serialize(request, new JsonSerializerOptions{IncludeFields = true});
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        
        return await client.PostAsync("adminItems", content);
    }

    public async Task<HttpResponseMessage> GetAdminItems()
    {
        var client = GetClient();
        return await client.GetAsync("adminItems");
    }

    public async Task<HttpResponseMessage> GetColors()
    {
        var client = GetClient();
        return await client.GetAsync("colors");
    }

    private HttpClient GetClient() => _client ??= CreateClient();
}