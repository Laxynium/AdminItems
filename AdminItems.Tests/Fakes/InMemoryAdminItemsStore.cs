using System.Collections.ObjectModel;
using AdminItems.Api.AdminItems;

namespace AdminItems.Tests.Fakes;

public class InMemoryAdminItemsStore : Collection<AdminItem>, IAdminItemsStore
{
    public Task Add(AdminItem adminItem)
    {
        base.Add(adminItem);
        
        return Task.CompletedTask;
    }

    public Task Update(long id, AdminItem adminItem)
    {
        base.SetItem((int)(id-1L), adminItem);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TResult>> GetAll<TResult, TProperty>(
        Func<AdminItem, TResult> mapper,
        Func<AdminItem, TProperty> orderer) => 
        Task.FromResult(Items.OrderBy(orderer).Select(mapper).ToList() as IReadOnlyList<TResult>);
}