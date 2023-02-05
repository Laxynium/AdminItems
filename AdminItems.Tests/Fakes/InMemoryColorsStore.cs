using AdminItems.Api.Colors;

namespace AdminItems.Tests.Fakes;

public class InMemoryColorsStore : IColorsStore
{
    private  Color[] _colors = Array.Empty<Color>();
    
    public void AddColors(Color[] colors)
    {
        _colors = colors;
    }

    public IReadOnlyList<ColorDto> GetAll(Func<Color, ColorDto> mapper) =>
        _colors
            .Select(mapper)
            .ToList();
}