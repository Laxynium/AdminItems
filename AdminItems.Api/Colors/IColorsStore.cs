namespace AdminItems.Api.Colors;

public interface IColorsStore
{
    Task<IReadOnlyList<TResult>> GetAll<TResult>(Func<Color, TResult> mapper);
}

internal sealed class NullColorsStore : IColorsStore
{
    public Task<IReadOnlyList<TResult>> GetAll<TResult>(Func<Color, TResult> mapper)
    {
        return Task.FromResult<IReadOnlyList<TResult>>(new List<TResult>());
    }
}