using AdminItems.Api.Framework;
using Dapper;
using Npgsql;

namespace AdminItems.Api.AdminItems;

public class AdminItemEntity : Entity<AdminItemId, long, AdminItem, AdminItemEntity>
{
    public AdminItemEntity(AdminItemId id, long version, AdminItem value) : base(id, version, value)
    {
    }
}
public interface IAdminItemsStore
{
    public Task Add(AdminItemId id, AdminItem adminItem);

    Task Update(AdminItemEntity entity);

    Task<AdminItemEntity?> Find(AdminItemId id);
}

internal sealed class SqlAdminItemsStore : IAdminItemsStore
{
    private readonly string _connectionString;

    public SqlAdminItemsStore(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task Add(AdminItemId id, AdminItem adminItem) =>
        await HandleInsertErrors(id, async () =>
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
        });

    public async Task Update(AdminItemEntity entity)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var result = await connection.ExecuteAsync(@"
UPDATE ""admin_items"" ai
SET 
    ""code"" = @Code,
    ""name"" =  @Name,
    ""comments"" = @Comments,
    ""color"" = @Color
WHERE ""id"" = @Id
AND ""xmin"" = cast(@Version as int)",
            new
            {
                Id = entity.Id.Value,
                Version = entity.Version,
                Code = entity.Value.Code,
                Name = entity.Value.Name,
                Comments = entity.Value.Comments,
                Color = entity.Value.Color
            });

        if (result == 0)
        {
            throw new OptimisticConcurrencyException("update", "AdminItem", entity.Id.ToString());
        }
    }

    public async Task<AdminItemEntity?> Find(AdminItemId id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var adminItemRecord = await connection
            .QuerySingleOrDefaultAsync<(long id, long version, string code, string name, string comments, string color
                )?>(@"
SELECT  ""id""
    ,   ""xmin"" 
    ,   ""code""
    ,   ""name""
    ,   ""comments""
    ,   ""color""
FROM ""admin_items""
WHERE ""id"" = @Id", new {Id = id.Value});

        if (adminItemRecord is null)
        {
            return null;
        }

        return new AdminItemEntity(id, adminItemRecord.Value.version, new AdminItem(
            adminItemRecord.Value.code,
            adminItemRecord.Value.name,
            adminItemRecord.Value.comments,
            adminItemRecord.Value.color));
    }

    private static async Task HandleInsertErrors(AdminItemId id, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            throw new OptimisticConcurrencyException("insertion", "AdminItem", id.ToString());
        }
    }
}