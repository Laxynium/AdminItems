using Dapper;
using Npgsql;

namespace AdminItems.Api.Identity;

public class SeedUsersHostService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly PasswordHasher _hasher;

    public SeedUsersHostService(IServiceProvider serviceProvider, PasswordHasher hasher)
    {
        _serviceProvider = serviceProvider;
        _hasher = hasher;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        await using var connection = scope.ServiceProvider.GetRequiredService<NpgsqlConnection>();
        
        var normalUser = UserEntity.Create("normalUser", _hasher.HashPassword("Password123!@"), new List<string>());
        await connection.ExecuteAsync(@"INSERT INTO ""users"" (""user"", ""hash"", ""roles"") VALUES (@User, @Hash, @Roles) ON CONFLICT DO NOTHING",new
        {
            User = normalUser.User,
            Hash = normalUser.Hash,
            Roles = normalUser.RolesAsString
        });
        
        var adminUser = UserEntity.Create("adminUser", _hasher.HashPassword("Password123!@"), new List<string>{"Admin-ManageItems"});
        await connection.ExecuteAsync(@"INSERT INTO ""users"" (""user"", ""hash"", ""roles"") VALUES (@User, @Hash, @Roles) ON CONFLICT DO NOTHING",new
        {
            User = adminUser.User,
            Hash = adminUser.Hash,
            Roles = adminUser.RolesAsString
        });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}