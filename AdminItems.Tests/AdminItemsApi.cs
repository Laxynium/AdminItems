using System.Text;
using System.Text.Json;
using AdminItems.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AdminItems.Tests;

public class AdminItemsApi : WebApplicationFactory<Program>
{
    private FakeAdminItemsStore _fakeAdminItemsStore;

    public void UseStore(FakeAdminItemsStore fakeAdminItemsStore)
    {
        _fakeAdminItemsStore = fakeAdminItemsStore;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAdminItemsStore>();
            services.TryAddSingleton<IAdminItemsStore>(_fakeAdminItemsStore);
        });
    }

    public async Task<HttpResponseMessage> PostAdminItem(object request)
    {
        var client = CreateClient();
        
        var serialized = JsonSerializer.Serialize(request);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        
        return await client.PostAsync("adminItems", content);
    }
}