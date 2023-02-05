using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api;

public record Response(List<ColorDto> Items);
public record ColorDto(long Id, string Name);

[ApiController]
[Route("[controller]")]
public class ColorsController : ControllerBase
{

    [HttpGet]
    public Task<Response> Get()
    {
        var response = new Response(new List<ColorDto>
        {
            new(1, "alizarin"),
            new(2, "amethyst"),
            new(3, "beige"),
            new(4, "cherry"),
        });
        return Task.FromResult(response);
    }
}