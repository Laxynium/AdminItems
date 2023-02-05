using System.Text;
using System.Text.Json;
using AdminItems.Api;
using AdminItems.Api.AdminItems;
using AdminItems.Api.Colors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AdminItems.Tests;

public class AdminItemsApi : WebApplicationFactory<Api.Program>
{
    private FakeAdminItemsStore _fakeAdminItemsStore = new();
    private FakeColorsStore _fakeColorsStore = new();

    public void UseStore(FakeAdminItemsStore fakeAdminItemsStore)
    {
        _fakeAdminItemsStore = fakeAdminItemsStore;
    }

    public void UseStore(FakeColorsStore fakeColorsStore)
    {
        _fakeColorsStore = fakeColorsStore;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAdminItemsStore>();
            services.TryAddSingleton<IAdminItemsStore>(_fakeAdminItemsStore);

            services.RemoveAll<IColorsStore>();
            services.TryAddSingleton<IColorsStore>(_fakeColorsStore);
        });
    }

    public async Task<HttpResponseMessage> PostAdminItem(object request)
    {
        var client = CreateClient();
        
        var serialized = JsonSerializer.Serialize(request);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        
        return await client.PostAsync("adminItems", content);
    }

    public async Task<HttpResponseMessage> GetColors()
    {
        var client = CreateClient();
        return await client.GetAsync("colors");
    }
}