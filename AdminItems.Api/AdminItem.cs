using CSharpFunctionalExtensions;

namespace AdminItems.Api;

public class AdminItem : ValueObject
{
    public string Code { get; }
    public string Name { get; }
    public string Comments { get; }

    public AdminItem(string code, string name, string comments)
    {
        Code = code;
        Name = name;
        Comments = comments;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
        yield return Name;
        yield return Comments;
    }
}