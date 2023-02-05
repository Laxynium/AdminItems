namespace AdminItems.Api;

public interface IAdminItemsStore
{
    public Task Add(AdminItem adminItem);
}

internal sealed class NullAdminItemsStore : IAdminItemsStore
{
    public Task Add(AdminItem adminItem)
    {
        return Task.CompletedTask;
    }
}