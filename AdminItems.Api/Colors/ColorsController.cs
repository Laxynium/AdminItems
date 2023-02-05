using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api.Colors;

public record Response(IReadOnlyList<ColorDto> Items);

public record ColorDto(long Id, string Name);

[ApiController]
[Route("[controller]")]
public class ColorsController : ControllerBase
{
    private readonly IColorsStore _colorsStore;

    public ColorsController(IColorsStore colorsStore)
    {
        _colorsStore = colorsStore;
    }

    [HttpGet]
    public async Task<Response> Get()
    {
        var colors = await _colorsStore.GetAll(x => new ColorDto(x.Id, x.Name));
        var response = new Response(colors);
        return response;
    }
}