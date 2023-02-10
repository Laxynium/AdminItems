using AdminItems.Api.AdminItems;

namespace AdminItems.Tests.Fakes;

public class InMemoryAdminItemsStore : Dictionary<AdminItemId, AdminItem>, IAdminItemsStore
{
    public Task Add(AdminItemId id, AdminItem adminItem)
    {
        base.Add(AdminItemId.Create(id), adminItem);

        return Task.CompletedTask;
    }


    public Task Update(AdminItemEntity entity)
    {
        if (ContainsKey(entity.Id))
        {
            base[entity.Id] = entity.Value;
        }
        return Task.CompletedTask;
    }

    public Task<AdminItemEntity?> Find(AdminItemId id)
    {
        if (ContainsKey(id))
        {
            var adminItem = base[id];
            return Task.FromResult((AdminItemEntity?)new AdminItemEntity(id, 1, adminItem));
        }
        return Task.FromResult((AdminItemEntity?)null);
    }
}