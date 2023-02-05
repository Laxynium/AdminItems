using AdminItems.Api.AdminItems;

namespace AdminItems.Tests.Fakes;

public class InMemoryAdminItemsStore : Dictionary<AdminItemId, AdminItem>, IAdminItemsStore
{
    public Task Add(long id, AdminItem adminItem)
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

    public Task<IReadOnlyList<TResult>> GetAll<TResult, TProperty>(
        Func<AdminItemId, AdminItem, TResult> mapper,
        Func<AdminItem, TProperty> orderer) => 
        Task.FromResult(Keys.Select(x => new { key = x, value = base[x] })
            .OrderBy(x => orderer(x.value))
            .Select(x => mapper(x.key, x.value))
            .ToList() as IReadOnlyList<TResult>);
}