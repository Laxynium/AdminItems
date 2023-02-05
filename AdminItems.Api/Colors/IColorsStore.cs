namespace AdminItems.Api.Colors;

public interface IColorsStore
{
    IReadOnlyList<TResult> GetAll<TResult>(Func<Color, TResult> mapper);
}

internal sealed class NullColorsStore : IColorsStore
{
    public IReadOnlyList<TResult> GetAll<TResult>(Func<Color, TResult> mapper)
    {
        return new List<TResult>();
    }
}