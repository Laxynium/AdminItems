using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
    private readonly FakeAdminItemIdGenerator _adminItemIdGenerator = new();
    
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
        builder.UseEnvironment("testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAdminItemsStore>();
            services.TryAddSingleton(_adminItemsStore);

            services.RemoveAll<IColorsStore>();
            services.TryAddSingleton(_colorsStore);

            services.RemoveAll<IAdminItemIdGenerator>();
            services.TryAddSingleton<IAdminItemIdGenerator>(_adminItemIdGenerator);
        });
    }

    public AdminItemsApi WillGenerateAdminItemId(params long[] ids)
    {
        foreach (var id in ids)
        {
            _adminItemIdGenerator.WillGenerate(id);    
        }
        return this;
    }
    
    public async Task<AdminItemId> ThereIsAnAdminItem(object request)
    {
        var response = await PostAdminItem(request);
        response.Should().Be201Created();
        var body = await response.Content.ReadFromJsonAsync<JsonNode>();
        return AdminItemId.Create(body!["id"]!.GetValue<long>());
    }

    public async Task<HttpResponseMessage> PostAdminItem(object request)
    {
        var client = GetClient();
        
        var serialized = JsonSerializer.Serialize(request);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        
        return await client.PostAsync("adminItems", content);
    }

    public async Task<HttpResponseMessage> PutAdminItem(long adminItemId, object request)
    {
        var client = GetClient();
        
        var serialized = JsonSerializer.Serialize(request);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        
        return await client.PutAsync($"adminItems/{adminItemId}", content);
    }

    private HttpClient GetClient() => _client ??= CreateClient();
}