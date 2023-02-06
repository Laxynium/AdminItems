using Dapper;
using Npgsql;

namespace AdminItems.Api.AdminItems;

public interface IAdminItemsStore
{
    public Task Add(long id, AdminItem adminItem);

    Task Update(AdminItemId id, AdminItem adminItem);

    Task<IReadOnlyList<TResult>> GetAll<TResult, TProperty>(
        Func<AdminItemId, AdminItem, TResult> mapper,
        Func<AdminItem, TProperty> orderer);

    Task<bool> Contains(AdminItemId id);
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

    public Task<bool> Contains(AdminItemId id)
    {
        return Task.FromResult(false);
    }
}

internal sealed class SqlAdminItemsStore : IAdminItemsStore
{
    private readonly string _connectionString;

    public SqlAdminItemsStore(string connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task Add(long id, AdminItem adminItem)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "INSERT INTO admin_items(id, code, name, comments, color) VALUES (@Id, @Code, @Name, @Comments, @Color)",
            new
            {
                Id = id, 
                Code = adminItem.Code, 
                Name = adminItem.Name, 
                Comments = adminItem.Comments,
                Color = adminItem.Color
            });
    }

    public  Task Update(AdminItemId id, AdminItem adminItem)
    {
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TResult>> GetAll<TResult, TProperty>(Func<AdminItemId, AdminItem, TResult> mapper, Func<AdminItem, TProperty> orderer)
    {
        return Task.FromResult(new List<TResult>() as IReadOnlyList<TResult>);
    }

    public Task<bool> Contains(AdminItemId id)
    {
        return Task.FromResult(false);
    }
}