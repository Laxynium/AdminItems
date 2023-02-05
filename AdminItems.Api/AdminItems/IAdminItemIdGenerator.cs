using IdGen;

namespace AdminItems.Api.AdminItems;

public interface IAdminItemIdGenerator
{
    public AdminItemId NextId();
}

public class IdGenAdminItemIdGenerator : IAdminItemIdGenerator
{
    private const int GeneratorId = 0;
    private readonly IdGenerator _generator = new(GeneratorId);
    public AdminItemId NextId()
    {
        var value = _generator.CreateId();
        return AdminItemId.Create(value);
    }
}