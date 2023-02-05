using CSharpFunctionalExtensions;

namespace AdminItems.Api;

public class Color : Entity<long>
{
    public string Name { get; }

    public Color(long id, string name):base(id)
    {
        Name = name;
    }
}