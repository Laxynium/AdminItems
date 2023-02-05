using System.Globalization;
using CSharpFunctionalExtensions;

namespace AdminItems.Api.AdminItems;

public class AdminItemId : SimpleValueObject<long>
{
    private AdminItemId(long value) : base(value)
    {
    }

    public static AdminItemId Create(long value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Admin item id must be >= 1");
        }

        return new AdminItemId(value);
    }

    public override string ToString() => 
        Value.ToString(CultureInfo.InvariantCulture);
}