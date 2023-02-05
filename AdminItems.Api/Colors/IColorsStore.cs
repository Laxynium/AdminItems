namespace AdminItems.Api.Colors;

public interface IColorsStore
{
    IReadOnlyList<ColorDto> GetAll(Func<Color, ColorDto> mapper);
}

internal sealed class NullColorsStore : IColorsStore
{
    public IReadOnlyList<ColorDto> GetAll(Func<Color, ColorDto> mapper) => new List<ColorDto>();
}