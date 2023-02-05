using CSharpFunctionalExtensions;

namespace AdminItems.Api.Colors;

public class Color : Entity<long>
{
    public Color(long id, string name) : base(id)
    {
        Name = name;
    }

    public string Name { get; }
}