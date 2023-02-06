using Dapper;
using Npgsql;

namespace AdminItems.Api.AdminItems;

public interface IAdminItemsStore
{
    public Task Add(AdminItemId id, AdminItem adminItem);

    Task Update(AdminItemId id, AdminItem adminItem);

    Task<IReadOnlyList<TResult>> GetAll<TResult, TProperty>(
        Func<AdminItemId, AdminItem, TResult> mapper,
        Func<AdminItem, TProperty> orderer);

    Task<bool> Contains(AdminItemId id);
}

internal sealed class NullAdminItemsStore : IAdminItemsStore
{
    public Task Add(AdminItemId id, AdminItem adminItem)
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

    public async Task Add(AdminItemId id, AdminItem adminItem)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            @"INSERT INTO ""admin_items"" (""id"", ""code"", ""name"", ""comments"", ""color"") VALUES (@Id, @Code, @Name, @Comments, @Color)",
            new
            {
                Id = id.Value,
                Code = adminItem.Code,
                Name = adminItem.Name,
                Comments = adminItem.Comments,
                Color = adminItem.Color
            });
    }

    public async Task Update(AdminItemId id, AdminItem adminItem)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(@"
UPDATE ""admin_items"" ai
SET 
    ""code"" = @Code,
    ""name"" =  @Name,
    ""comments"" = @Comments,
    ""color"" = @Color
WHERE ai.id = @Id",
            new
            {
                Id = id.Value,
                Code = adminItem.Code,
                Name = adminItem.Name,
                Comments = adminItem.Comments,
                Color = adminItem.Color
            });
    }

    public Task<IReadOnlyList<TResult>> GetAll<TResult, TProperty>(Func<AdminItemId, AdminItem, TResult> mapper,
        Func<AdminItem, TProperty> orderer)
    {
        return Task.FromResult(new List<TResult>() as IReadOnlyList<TResult>);
    }

    public async Task<bool> Contains(AdminItemId id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var result = await connection.ExecuteScalarAsync<bool>(
            @"SELECT 1 FROM ""admin_items"" ai WHERE ai.id = @Id",
            new { Id = id.Value });
        return result;
    }
}