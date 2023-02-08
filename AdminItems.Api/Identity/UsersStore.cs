using Dapper;
using Npgsql;

namespace AdminItems.Api.Identity;

public class UsersStore
{
    private readonly NpgsqlConnection _connection;

    public UsersStore(NpgsqlConnection connection)
    {
        _connection = connection;
    }
    
    public async Task<UserEntity?> Find(string user)
    {
        var result = await _connection.QuerySingleOrDefaultAsync<UserEntity>(@"
SELECT ""id"", ""user"", ""hash"", ""roles"" 
FROM ""users""
WHERE ""user"" = @User", new {User = user});

        return result;
    }
}