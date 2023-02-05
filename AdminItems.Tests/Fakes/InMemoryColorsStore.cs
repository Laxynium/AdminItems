using AdminItems.Api.Colors;

namespace AdminItems.Tests.Fakes;

public class InMemoryColorsStore : IColorsStore
{
    private  Color[] _colors = Array.Empty<Color>();
    
    public void AddColors(Color[] colors)
    {
        _colors = colors;
    }

    public Task<IReadOnlyList<TResult>> GetAll<TResult>(Func<Color, TResult> mapper) =>
        Task.FromResult(_colors
            .Select(mapper)
            .ToList() as IReadOnlyList<TResult>);
}