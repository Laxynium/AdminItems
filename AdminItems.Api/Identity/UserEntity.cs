using System.Text.Json;
using CSharpFunctionalExtensions;

namespace AdminItems.Api.Identity;

public class UserEntity : Entity<long>
{
    public string User { get;  }
    public string Hash { get; }
    public List<string> Roles { get; } = new();
    public string RolesAsString => JsonSerializer.Serialize(Roles);

    private UserEntity(long id, string user, string hash, string roles):base(id)
    {
        User = user.ToLowerInvariant();
        Hash = hash;
        Roles = JsonSerializer.Deserialize<List<string>>(roles);
    }

    public static UserEntity Create(string user, string hash, List<string> roles) =>
        new(0, user, hash, JsonSerializer.Serialize(roles));
}