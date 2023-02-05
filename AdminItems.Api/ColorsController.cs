using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api;

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
    public Task<Response> Get()
    {
        var colors = _colorsStore.GetAll(x => new ColorDto(x.Id, x.Name));
        var response = new Response(colors);
        return Task.FromResult(response);
    }
}