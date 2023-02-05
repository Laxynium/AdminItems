using System.Collections.ObjectModel;
using AdminItems.Api;
using AdminItems.Api.AdminItems;

namespace AdminItems.Tests;

public class FakeAdminItemsStore : Collection<AdminItem>, IAdminItemsStore
{
    public Task Add(AdminItem adminItem)
    {
        base.Add(adminItem);
        
        return Task.CompletedTask;
    }
}