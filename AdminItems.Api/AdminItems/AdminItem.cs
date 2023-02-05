using CSharpFunctionalExtensions;

namespace AdminItems.Api.AdminItems;

public class AdminItem : ValueObject
{
    public AdminItem(string code, string name, string comments, string color)
    {
        Code = code;
        Name = name;
        Comments = comments;
        Color = color;
    }

    public string Code { get; }
    public string Name { get; }
    public string Comments { get; }
    public string Color { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
        yield return Name;
        yield return Comments;
        yield return Color;
    }
}