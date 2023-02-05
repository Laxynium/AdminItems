namespace AdminItems.Api.AdminItems;

public interface IAdminItemsStore
{
    public Task Add(long id, AdminItem adminItem);

    Task Update(AdminItemId id, AdminItem adminItem);

    Task<IReadOnlyList<TResult>> GetAll<TResult, TProperty>(
        Func<AdminItemId, AdminItem, TResult> mapper,
        Func<AdminItem, TProperty> orderer);
}

internal sealed class NullAdminItemsStore : IAdminItemsStore
{
    public Task Add(long id, AdminItem adminItem)
    {
        return Task.CompletedTask;
    }

    public Task Update(AdminItemId id, AdminItem adminItem)
    {
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TResult>> GetAll<TResult, TProperty>(Func<AdminItemId, AdminItem, TResult> mapper,
        Func<AdminItem, TProperty> orderer) => 
        Task.FromResult(new List<TResult>() as IReadOnlyList<TResult>);
}