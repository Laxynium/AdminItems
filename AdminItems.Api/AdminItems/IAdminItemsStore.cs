namespace AdminItems.Api.AdminItems;

public interface IAdminItemsStore
{
    public Task Add(AdminItem adminItem);

    Task Update(long id, AdminItem adminItem);

    Task<IReadOnlyList<TResult>> GetAll<TResult, TProperty>(
        Func<AdminItem, TResult> mapper,
        Func<AdminItem, TProperty> orderer);
}

internal sealed class NullAdminItemsStore : IAdminItemsStore
{
    public Task Add(AdminItem adminItem)
    {
        return Task.CompletedTask;
    }

    public Task Update(long id, AdminItem adminItem)
    {
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TResult>> GetAll<TResult, TProperty>(Func<AdminItem, TResult> mapper,
        Func<AdminItem, TProperty> orderer) => 
        Task.FromResult(new List<TResult>() as IReadOnlyList<TResult>);
}