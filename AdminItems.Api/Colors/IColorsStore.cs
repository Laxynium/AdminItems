namespace AdminItems.Api.Colors;

public interface IColorsStore
{
    Task<Color?> Find(long colorId);
    Task<IReadOnlyList<TResult>> GetAll<TResult>(Func<Color, TResult> mapper);
}

internal sealed class NullColorsStore : IColorsStore
{
    public Task<Color?> Find(long colorId) => Task.FromResult((Color?)null);

    public Task<IReadOnlyList<TResult>> GetAll<TResult>(Func<Color, TResult> mapper) => 
        Task.FromResult<IReadOnlyList<TResult>>(new List<TResult>());
}