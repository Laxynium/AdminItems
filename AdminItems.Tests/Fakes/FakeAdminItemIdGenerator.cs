using AdminItems.Api.AdminItems;

namespace AdminItems.Tests.Fakes;

public class FakeAdminItemIdGenerator : IAdminItemIdGenerator
{
    private readonly FakeIdGenerator<AdminItemId> _generator = new();
    
    public AdminItemId NextId()
    {
        return _generator.Next();
    }

    public void WillGenerate(long id)
    {
        _generator.Enqueue(AdminItemId.Create(id));
    }
}