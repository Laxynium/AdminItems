using CSharpFunctionalExtensions;

namespace AdminItems.Api.Colors;

public class Color : Entity<long>
{
    public string Name { get; }

    public Color(long id, string name):base(id)
    {
        Name = name;
    }
}