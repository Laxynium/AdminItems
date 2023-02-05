using System.Collections.ObjectModel;
using AdminItems.Api.AdminItems;

namespace AdminItems.Tests;

public class InMemoryAdminItemsStore : Collection<AdminItem>, IAdminItemsStore
{
    public Task Add(AdminItem adminItem)
    {
        base.Add(adminItem);
        
        return Task.CompletedTask;
    }
}