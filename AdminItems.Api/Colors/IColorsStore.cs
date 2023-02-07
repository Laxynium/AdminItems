using Dapper;
using Npgsql;

namespace AdminItems.Api.Colors;

public interface IColorsStore
{
    Task<Color?> Find(long colorId);
}

internal sealed class SqlColorStore : IColorsStore
{
    private readonly string _connectionString;

    public SqlColorStore(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<Color?> Find(long colorId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var color = await connection.QueryFirstOrDefaultAsync<Color>(@"
SELECT ""id"", ""name"" 
FROM ""colors"" c 
WHERE ""id"" = @Id", new {Id = colorId});
        
        return color;
    }
}