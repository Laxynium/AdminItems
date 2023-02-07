using AdminItems.Api.Colors;

namespace AdminItems.Tests.Fakes;

public class InMemoryColorsStore : IColorsStore
{
    private  Color[] _colors = Array.Empty<Color>();
    
    public void AddColors(Color[] colors)
    {
        _colors = colors;
    }

    public Task<Color?> Find(long colorId) => Task.FromResult(_colors.FirstOrDefault(x=>x.Id == colorId));
}