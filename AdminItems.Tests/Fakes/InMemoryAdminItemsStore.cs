using AdminItems.Api.AdminItems;

namespace AdminItems.Tests.Fakes;

public class InMemoryAdminItemsStore : Dictionary<AdminItemId, AdminItem>, IAdminItemsStore
{
    public Task Add(AdminItemId id, AdminItem adminItem)
    {
        base.Add(AdminItemId.Create(id), adminItem);

        return Task.CompletedTask;
    }

    public Task Update(AdminItemId id, AdminItem adminItem)
    {
        if (ContainsKey(AdminItemId.Create(id)))
        {
            base[AdminItemId.Create(id)] = adminItem;
        }
        return Task.CompletedTask;
    }

    public Task<bool> Contains(AdminItemId id) => 
        Task.FromResult(ContainsKey(id));
}