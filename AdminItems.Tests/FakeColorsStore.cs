using AdminItems.Api;

namespace AdminItems.Tests;

public class FakeColorsStore : IColorsStore
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