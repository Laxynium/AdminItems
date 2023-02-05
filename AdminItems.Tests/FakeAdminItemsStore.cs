using System.Collections.ObjectModel;
using AdminItems.Api;

namespace AdminItems.Tests;

public class FakeAdminItemsStore : Collection<AdminItem>, IAdminItemsStore
{
    public Task Add(AdminItem adminItem)
    {
        base.Add(adminItem);
        
        return Task.CompletedTask;
    }
}